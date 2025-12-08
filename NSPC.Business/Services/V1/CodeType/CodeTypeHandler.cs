using NSPC.Common;
using NSPC.Data;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Http;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.CodeType;
using NSPC.Data.Data.Entity.EInvoice;
using NSPC.Business.Services.EInvoice;

namespace NSPC.Business
{
    public class CodeTypeHandler : ICodeTypeHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;



        public CodeTypeHandler(SMDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var CodeType = await _dbContext
                    .sm_CodeType
                    .Include(x => x.sm_CodeType_Translation)
                    .Include(x => x.CodeTypeItems.OrderBy(x => x.LineNumber))
                    .Where(a => a.Id == id).FirstOrDefaultAsync();

                // UpdatePaid CodeType if exist
                if (CodeType == null)
                    return new Response(HttpStatusCode.NotFound, "Không tìm thấy bản ghi!");

                var result = _mapper.Map<sm_CodeType, CodeTypeViewModel>(CodeType);

                return new Response<CodeTypeViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: Id: {@param}", id);
                return new ResponseError(HttpStatusCode.InternalServerError,
                    "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> CreateCodeType(CodeTypeCreateUpdateModel param)
        {
            try
            {
                // Auto fill english title
                /*var defaultTranslation = param.Translations.Where(x => x.Language == LanguageConstants.Default).FirstOrDefault();*/

                /*if (string.IsNullOrEmpty(param.Title) && defaultTranslation != null)
                    param.Title = defaultTranslation.Title;*/

                var code = await _dbContext.sm_CodeType.Where(a => a.Code == param.Code).FirstOrDefaultAsync();
                if (code != null)
                    return Helper.CreateBadRequestResponse<CodeTypeCreateUpdateModel>("Code đã tồn tại");

                if (param.Code == null)
                    return Helper.CreateBadRequestResponse<CodeTypeCreateUpdateModel>("Code không được bỏ trống");

                var codeType = await _dbContext.sm_CodeType.Where(a => a.Code == a.Code && a.Type == param.Type)
                    .FirstOrDefaultAsync();

                if (param.CodeTypeItems != null && param.CodeTypeItems.Count > 0)
                {
                    // Check danh sách items không được trùng nhau
                    var checkDuplicateCodeItems = param.CodeTypeItems.GroupBy(x => x.Code).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (checkDuplicateCodeItems.Count > 0)
                        return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Danh mục con không được có mã trùng nhau.");

                    var checkDuplicateTitleItems = param.CodeTypeItems.GroupBy(x => x.Title).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (checkDuplicateTitleItems.Count > 0)
                        return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Danh mục con không được có tên trùng nhau.");

                    #region Tính toán số thứ tự items
                    param.CodeTypeItems = param.CodeTypeItems.OrderBy(x => x.LineNumber).ToList();
                    for (int i = 0; i < param.CodeTypeItems.Count; i++)
                        param.CodeTypeItems[i].LineNumber = i + 1;
                    #endregion
                }
                //if (codeType != null)
                //{
                //    return new ResponseError(HttpStatusCode.BadRequest, "Bản ghi đã tồn tại!");
                //}

                /*if (param.Translations?.Count > 0)
                {
                    var languages = param.Translations.Select(x => x.Language).ToList();
                    foreach (var language in languages)
                    {
                        if (string.IsNullOrEmpty(language))
                            return new ResponseError(HttpStatusCode.BadRequest, "Translations have invalid language code, should be one of: " + string.Join(", ", _availableLanguages));
                    }
                }*/

                codeType = new sm_CodeType();
                codeType.Id = Guid.NewGuid();
                codeType.ParentId = param.ParentId;
                codeType.Title = param.Title;
                codeType.Description = param.Description;
                codeType.Type = param.Type;
                codeType.Code = param.Code;
                codeType.Order = param.Order;
                codeType.IconClass = param.IconClass;
                codeType.CodeTypeItems = new List<sm_CodeType_Item>();

                foreach (var modelItem in param.CodeTypeItems)
                {
                    // Readd new item
                    var item = _mapper.Map<sm_CodeType_Item>(modelItem);
                    codeType.CodeTypeItems.Add(item);
                }

                codeType.CreatedOnDate = DateTime.Now;

                /*if (param.Translations?.Count > 0)
                {
                    codeType.TranslationCount = param.Translations.Count();
                    codeType.ub_CodeType_Translation = new List<ub_CodeType_Translation>();
                    foreach (var itemTranslations in param.Translations)
                    {
                        var newCodeTypeTranslations = new ub_CodeType_Translation();
                        newCodeTypeTranslations.CodeTypeId = codeType.Id;
                        newCodeTypeTranslations.Title = itemTranslations.Title;
                        newCodeTypeTranslations.Type = codeType.Type;
                        newCodeTypeTranslations.Language = itemTranslations.Language;
                        newCodeTypeTranslations.Description = itemTranslations.Description;
                        codeType.ub_CodeType_Translation.Add(newCodeTypeTranslations);
                    }
                }*/

                _dbContext.Add(codeType);


                var status = await _dbContext.SaveChangesAsync();
                // Reload collection
                CodeTypeCollection.Instance.LoadData();

                var data = _mapper.Map<CodeTypeViewModel>(codeType);

                return new Response<CodeTypeViewModel>(HttpStatusCode.OK, data, "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: {@Param}", param);
                return new Response<CodeTypeViewModel>
                {
                    Data = null,
                    Message = ex.Message,
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<Response> UpdateCodeType(CodeTypeCreateUpdateModel param, Guid id)
        {
            {
                try
                {
                    var codeType = await _dbContext.sm_CodeType.Where(a => a.Id == id).Include(a => a.CodeTypeItems).FirstOrDefaultAsync();
                    if (codeType == null)
                        return Helper.CreateNotFoundResponse<string>("Danh mục không tồn tại trong hệ thống");

                    if (param.CodeTypeItems != null && param.CodeTypeItems.Count > 0)
                    {
                        // Check danh sách items không được trùng nhau
                        var checkDuplicateCodeItems = param.CodeTypeItems.GroupBy(x => x.Code).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (checkDuplicateCodeItems.Count > 0)
                            return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Danh mục con không được có mã trùng nhau.");

                        var checkDuplicateTitleItems = param.CodeTypeItems.GroupBy(x => x.Title).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (checkDuplicateTitleItems.Count > 0)
                            return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Danh mục con không được có tên trùng nhau.");

                        #region Tính toán số thứ tự items
                        param.CodeTypeItems = param.CodeTypeItems.OrderBy(x => x.LineNumber).ToList();
                        for (int i = 0; i < param.CodeTypeItems.Count; i++)
                            param.CodeTypeItems[i].LineNumber = i + 1;
                        #endregion
                    }

                    //var code = await _dbContext.sm_CodeType.Where(a => a.Code == param.Code).FirstOrDefaultAsync();
                    //if (code != null)
                    //    return Helper.CreateBadRequestResponse<CodeTypeCreateUpdateModel>("Code đã tồn tại");

                    /*if (param.Translations?.Count > 0)
                    {
                        var languages = param.Translations.Select(x => x.Language).ToList();
                        foreach (var language in languages)
                        {
                            if (string.IsNullOrEmpty(language))
                                return new ResponseError(HttpStatusCode.BadRequest, "Translations have invalid language code, should be one of: " + string.Join(", ", _availableLanguages));
                        }
                    }*/
                    // UpdatePaid
                    codeType.Id = id;
                    codeType.ParentId = param.ParentId;
                    codeType.Title = param.Title;
                    codeType.Description = param.Description;
                    codeType.CreatedOnDate = codeType.CreatedOnDate;
                    codeType.Type = codeType.Type;
                    codeType.IconClass = param.IconClass;
                    codeType.LastModifiedOnDate = DateTime.Now;
                    //codeType.Code = param.Code;
                    codeType.Order = param.Order;

                    _dbContext.RemoveRange(codeType.CodeTypeItems);
                    codeType.CodeTypeItems = new List<sm_CodeType_Item>();

                    if (param.CodeTypeItems != null && param.CodeTypeItems.Count > 0)
                    {
                        foreach (var modelItem in param.CodeTypeItems)
                        {
                            // Readd new item
                            var item = _mapper.Map<sm_CodeType_Item>(modelItem);
                            codeType.CodeTypeItems.Add(item);
                        }
                    }
                    //codeType.CodeTypeItems = new List<sm_CodeType_Item>();

                    //foreach (var modelItem in param.CodeTypeItems)
                    //{
                    //    // Readd new item
                    //    var item = _mapper.Map<sm_CodeType_Item>(modelItem);
                    //    codeType.CodeTypeItems.Add(item);
                    //}

                    /*if (param.Translations != null && param.Translations.Any())
                    {
                        foreach (var translationModel in param.Translations)
                        {
                            bool isUpdateTranslation = true;

                            // Select translation
                            var translation = await _dbContext.ub_CodeType_Translation
                                .FirstOrDefaultAsync(a => a.CodeTypeId == id && a.Language == translationModel.Language);

                            if (translation == null)
                            {
                                translation = new ub_CodeType_Translation();
                                translation.Id = Guid.NewGuid();
                                translation.CreatedOnDate = DateTime.Now;
                                isUpdateTranslation = false;
                            }

                            translation.CodeTypeId = codeType.Id;
                            translation.Title = translationModel.Title;
                            translation.Description = translationModel.Description;
                            translation.Language = translationModel.Language;
                            translation.Type = codeType.Type;
                            translation.LastModifiedOnDate = DateTime.Now;

                            if (!isUpdateTranslation)
                                _dbContext.ub_CodeType_Translation.Add(translation);
                        }
                    }*/

                    _dbContext.sm_CodeType.Update(codeType);

                    await _dbContext.SaveChangesAsync();

                    // Reload collection
                    CodeTypeCollection.Instance.LoadData();

                    //var data = _mapper.Map<sm_CodeType, CodeTypeViewModel>(codeType);
                    //return new Response<CodeTypeViewModel>(HttpStatusCode.OK, data, "Chỉnh sửa thành công");

                    var result = await GetById(codeType.Id);

                    if (result.IsSuccess)
                        result.Message = "Chỉnh sửa danh mục thành công.";

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, string.Empty);
                    Log.Information("Params: {@params}, Ids: {@Ids}", param, id);
                    return Utils.CreateExceptionResponseError(ex);
                }
            }
        }

        public async Task<Response> Delete(Guid id)
        {
            try
            {
                var ct = await _dbContext.sm_CodeType.Include(x => x.sm_CodeType_Translation).Where(a => a.Id == id)
                    .FirstOrDefaultAsync();

                if (ct == null)
                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy bản ghi");

                _dbContext.sm_CodeType.Remove(ct);

                await _dbContext.SaveChangesAsync();

                // Reload collection
                CodeTypeCollection.Instance.LoadData();

                return new ResponseDelete(HttpStatusCode.OK, "Xóa thành công", id, "");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetAdminPageAsync(CodeTypeQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_CodeType
                    .Include(x => x.sm_CodeType_Translation)
                    .Include(x => x.CodeTypeItems.OrderBy(x => x.LineNumber))
                    .AsNoTracking()
                    .Where(predicate).OrderBy(x => x.CreatedOnDate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<sm_CodeType>, Pagination<CodeTypeViewModel>>(data);

                if (result != null)
                {
                    // Process language
                    if (!string.IsNullOrEmpty(query.Language))
                    {
                        foreach (var item in result.Content)
                        {
                            //item.Title = item.Translations.Where(x => x.Language == query.Language).FirstOrDefault()?.Title;
                            //item.Description = item.Translations.Where(x => x.Language == query.Language).FirstOrDefault()?.Description;
                            //item.Translations = null;
                        }
                    }

                    return new ResponsePagination<CodeTypeViewModel>(result);
                }

                return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@params}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }


        public async Task<Response<Pagination<CodeTypeListModel>>> GetListPageAsync(CodeTypeQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_CodeType
                    .Include(x => x.CodeTypeItems.OrderBy(x => x.LineNumber))
                    .Where(predicate).OrderBy(x => x.CreatedOnDate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<sm_CodeType>, Pagination<CodeTypeListModel>>(data);

                // Add ALL option
                //if (query.Format == "radio")
                //{
                //    result.Content.Insert(0, new CodeTypeListModel
                //    {
                //        Code = "ALL",
                //        Id = Guid.Empty,
                //        Title = "All",
                //        Translations = new List<CodeTypeTranslationModel>
                //        {
                //            new CodeTypeTranslationModel { Title = "All", Description = "", Language = "en" },
                //            new CodeTypeTranslationModel { Title = "Tất cả", Description = "", Language = "vn" },
                //            new CodeTypeTranslationModel { Title = "全て", Description = "", Language = "jp" },
                //            new CodeTypeTranslationModel { Title = "全部", Description = "", Language = "zh" },
                //        }
                //    });
                //}

                if (result != null)
                {
                    /*// Process language
                    if (!string.IsNullOrEmpty(query.Language))
                    {
                        foreach (var item in result.Content)
                        {
                            item.Title = item.Translations.Where(x => x.Language == query.Language).FirstOrDefault()
                                ?.Title;
                            // var translation = item.Translations.FirstOrDefault(x => x.Language == query.Language);
                            // if (translation != null)
                            // {
                            //     item.Title = translation.Title;
                            // }
                        }
                    }*/
                    return new Response<Pagination<CodeTypeListModel>>(result);
                }

                return Helper.CreateNotFoundResponse<Pagination<CodeTypeListModel>>("Không tìm thấy dữ liệu");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query {@params}", query);
                return Helper.CreateExceptionResponse<Pagination<CodeTypeListModel>>(ex);
            }
        }

        public async Task<Response> CreateCustomerLabel(CustomerLabelCreateUpdateModel param)
        {
            try
            {
                // Auto fill english title
                /*var defaultTranslation = param.Translations.Where(x => x.Language == LanguageConstants.Default).FirstOrDefault();*/

                /*if (string.IsNullOrEmpty(param.Title) && defaultTranslation != null)
                    param.Title = defaultTranslation.Title;*/
                var codeType = new sm_CodeType();
                codeType = new sm_CodeType();
                codeType.Id = Guid.NewGuid();
                codeType.Title = param.Title;
                codeType.Order = 0;
                codeType.CreatedOnDate = DateTime.Now;


                _dbContext.Add(codeType);


                var status = await _dbContext.SaveChangesAsync();
                // Reload collection
                CodeTypeCollection.Instance.LoadData();

                var data = _mapper.Map<CodeTypeViewModel>(codeType);
                CodeTypeCollection.Instance.LoadData();
                return new Response<CodeTypeViewModel>(HttpStatusCode.OK, data, "Thêm loại khách hàng thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: {@Param}", param);
                return new Response<CodeTypeViewModel>
                {
                    Data = null,
                    Message = ex.Message,
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<Response> UpdateCustomerLabel(CustomerLabelCreateUpdateModel param, string labelCode)
        {
            try
            {
                var codeType = await _dbContext.sm_CodeType.Where(a => a.Code == labelCode).FirstOrDefaultAsync();

                if (codeType == null)
                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy loại khách hàng");


                // UpdatePaid
                codeType.Title = param.Title;
                codeType.LastModifiedOnDate = DateTime.Now;


                await _dbContext.SaveChangesAsync();

                // Reload collection
                CodeTypeCollection.Instance.LoadData();

                var data = _mapper.Map<sm_CodeType, CodeTypeViewModel>(codeType);
                CodeTypeCollection.Instance.LoadData();
                return new Response<CodeTypeViewModel>(HttpStatusCode.OK, data, "Sửa loại khách hàng thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@param}, Label: {@labelCode}", param, labelCode);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetByLabelCode(string labelCode)
        {
            try
            {
                var CodeType = await _dbContext.sm_CodeType.Include(x => x.sm_CodeType_Translation)
                    .Where(a => a.Code == labelCode).FirstOrDefaultAsync();

                // UpdatePaid CodeType if exist
                if (CodeType == null)
                    return new Response(HttpStatusCode.NotFound, "Không tìm thấy bản ghi!");

                var result = _mapper.Map<sm_CodeType, CodeTypeViewModel>(CodeType);

                return new Response<CodeTypeViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: Code: {@param}", labelCode);
                return new ResponseError(HttpStatusCode.InternalServerError,
                    "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        private Expression<Func<sm_CodeType, bool>> BuildQuery(CodeTypeQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            
            var predicate = PredicateBuilder.New<sm_CodeType>(true);

            if (query.FromDate.HasValue)
            {
                predicate.And(x => x.CreatedOnDate >= query.FromDate.Value);
            }

            if (query.ToDate.HasValue)
            {
                predicate.And(x => x.CreatedOnDate <= query.ToDate.Value);
            }

            if (!string.IsNullOrEmpty(query.Type))
            {
                predicate.And(x => x.Type == query.Type);
            }
            if (!string.IsNullOrEmpty(query.Code))
            {
                predicate.And(x => x.Code == query.Code);
            }

            if (!string.IsNullOrEmpty(query.FullTextSearch))
            {
                predicate.And(x => x.Description.ToLower().Contains(query.FullTextSearch.ToLower()) ||
                                   x.Title.ToLower().Contains(query.FullTextSearch.ToLower()) ||
                                   x.Code.ToLower().Contains(query.FullTextSearch.ToLower()) ||
                                   x.sm_CodeType_Translation.Any(c => c.Description.Contains(query.FullTextSearch))
                );
            }

            if (query.ParentId.HasValue)
            {
                predicate.And(x => x.ParentId == query.ParentId.Value);
            }

            if (query.IsChildrenCustomer.HasValue)
            {
                if (query.IsChildrenCustomer.Value == true)
                {
                    predicate.And(x => x.Code != "R48");
                }
                else
                {
                    predicate.And(x => x.Code != "R17");
                }
            }

            return predicate;
        }
    }
}