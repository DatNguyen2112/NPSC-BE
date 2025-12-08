using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NSPC.Business.Services.Cata;
using NSPC.Data.Data.Entity.VatTu;
using NSPC.Data.Entity;
using static NSPC.Common.Helper;
using NSPC.Business.Services.ConstructionActitvityLog;

namespace NSPC.Business.Services
{
    public class MaterialRequestHandler : IMaterialRequestHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        private readonly IConstructionActivityLogHandler  _constructionActivityLogHandler;

        public MaterialRequestHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IConstructionActivityLogHandler constructionActivityLogHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _constructionActivityLogHandler = constructionActivityLogHandler;
        }

        /// <summary>
        /// Thêm mới yêu cầu vật tư
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<MaterialRequestViewModel>> Create(MaterialRequestCreateUpdateModel model,
            RequestUser currentUser)
        {
            try
            {
                List<jsonb_HistoryProcess> allHistoryProcess = new List<jsonb_HistoryProcess>();
                
                List<sm_Product> allConstructionItemProducts = new List<sm_Product>();

                #region Validate
                //Validate Product Item Model
                //1.Product ton tai trong he thong
                //2.SL yêu cầu vao khong duoc < 0
                //6.Khong duoc duplicate dong trong Product Item

                if (model.MaterialRequestItems != null && model.MaterialRequestItems.Count > 0)
                {
                    // Fill tat ca products
                    var allProductIds = model.MaterialRequestItems.Select(x => x.ProductId).ToList();
                    allConstructionItemProducts = await _dbContext.sm_Product.AsNoTracking()
                                            .Where(x => allProductIds.Contains(x.Id))
                                            .ToListAsync();

                    // Validate: Khong duoc duplicate dong trong Product Item
                    // Neu so luong distinct product id khac so luong product item => co dong trung
                    if (model.MaterialRequestItems.Count !=
                        model.MaterialRequestItems.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<MaterialRequestViewModel>
                                    ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: giá nhập và số lượng không được nhỏ hơn 0
                    if (model.MaterialRequestItems.Any(x => x.RequestQuantity < 0))
                        return Helper.CreateBadRequestResponse<MaterialRequestViewModel>
                                    ("Danh sách sản phẩm không được có số lượng nhỏ hơn 0.");

                    foreach (var item in model.MaterialRequestItems)
                    {
                        var product = allConstructionItemProducts
                                            .Where(x => x.Id == item.ProductId)
                                            .FirstOrDefault();
                        if (product == null)
                            return Helper.CreateBadRequestResponse<MaterialRequestViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");
                    }
                }
                #endregion

                #region Fill Item's Line No

                if (model.MaterialRequestItems != null && model.MaterialRequestItems.Count > 0)
                {
                    model.MaterialRequestItems = model.MaterialRequestItems.OrderBy(x => x.LineNo).ToList();
                    for (int i = 0; i < model.MaterialRequestItems.Count; i++)
                        model.MaterialRequestItems[i].LineNo = i + 1;
                }
                #endregion
                
                var entity = _mapper.Map<sm_MaterialRequest>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.FullName;
                entity.CreatedOnDate = DateTime.Now;
                entity.Code = await GetNewCode(MaterialRequestConstants.PrefixCode);
                entity.StatusCode = MaterialRequestConstants.StatusCode.DRAFT;
                entity.StatusName = MaterialRequestConstants.FetchStatus(MaterialRequestConstants.StatusCode.DRAFT).Name;
                
                #region Tính toán OrderItem
                foreach (var orderItem in entity.MaterialRequestItems)
                {
                    var product = allConstructionItemProducts.FirstOrDefault(x => x.Id == orderItem.ProductId);

                    if (product != null)
                    {
                        orderItem.MaterialRequestId = entity.Id;
                        orderItem.ConstructionId = entity.ConstructionId;
                        orderItem.IsApprove = false;
                        orderItem.Code = product.Code;
                        orderItem.Name = product.Name;
                        orderItem.Unit = product.Unit;
                        orderItem.PlannedQuantity = _dbContext.sm_ConstructionItems
                            .FirstOrDefault(x => x.ConstructionId == entity.ConstructionId && x.ProductId == orderItem.ProductId)
                            .PlannedQuantity;
                    }
                }
                #endregion
                
                #region Cập nhật lịch sử xử lý
                if (entity.ListHistoryProcess == null) {
                    allHistoryProcess.Add(new jsonb_HistoryProcess()
                    {
                        UserName = currentUser.FullName,
                        Description = "đã tạo yêu cầu vật tư",
                        CreatedOnDate = DateTime.Now,
                    });
                    
                    entity.ListHistoryProcess = allHistoryProcess;
                }
                #endregion

                
                _dbContext.sm_MaterialRequest.Add(entity);
                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động thêm mới yêu cầu vật tư vào bảng sm_ConstructionActivityLog
                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã tạo yêu cầu vật tư",
                        CodeLinkDescription = $"{entity.Code} - {entity.Content}",
                        OrderId =  entity.Id,
                        ConstructionId = entity.ConstructionId,
                    }, currentUser);
                    #endregion
                }
                
                return Helper.CreateSuccessResponse<MaterialRequestViewModel>(_mapper.Map<MaterialRequestViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                return Helper.CreateExceptionResponse<MaterialRequestViewModel>(ex);
            }
        }
        
        private async Task<string> GetNewCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_MaterialRequest.AsNoTracking().Where(x => x.Code.Contains(code)).OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();

                if (result != null)
                {
                    var currentNum = result.Code.Substring(result.Code.Length - 3, 3);
                    var currentNumInt = int.Parse(currentNum) + 1;
                    var stringResult = "";
                    if (currentNumInt < 10)
                    {
                        stringResult = "00" + currentNumInt;
                    }
                    else if (currentNumInt >= 10 && currentNumInt < 100)
                    {
                        stringResult = "0" + currentNumInt;
                    }
                    else
                    {
                        stringResult = currentNumInt.ToString();
                    }

                    return code + stringResult;
                }
                else
                {
                    return code + "001";
                }
            }
            catch (Exception ex)
            {
                Log.Error("123", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Cập nhật yêu cầu vật tư
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<MaterialRequestViewModel>> Update(Guid id, MaterialRequestCreateUpdateModel model,
            RequestUser currentUser)
        {
            try
            { 
                List<jsonb_HistoryProcess> allHistoryProcess = new List<jsonb_HistoryProcess>();
                
                List<sm_Product> allConstructionItemProducts = new List<sm_Product>();
                
                var entity =  await _dbContext.sm_MaterialRequest
                    .Include(x => x.Construction)
                    .Include(x => x.MaterialRequestItems)
                    .ThenInclude(x => x.sm_Product)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return Helper.CreateBadRequestResponse<MaterialRequestViewModel>("Không tìm thấy bản ghi");
                }

                #region Validate
                //Validate Product Item Model
                //1.Product ton tai trong he thong
                //2.SL yêu cầu vao khong duoc < 0
                //6.Khong duoc duplicate dong trong Product Item

                if (model.MaterialRequestItems != null && model.MaterialRequestItems.Count > 0)
                {
                    // Fill tat ca products
                    var allProductIds = model.MaterialRequestItems.Select(x => x.ProductId).ToList();
                    allConstructionItemProducts = await _dbContext.sm_Product.AsNoTracking()
                                            .Where(x => allProductIds.Contains(x.Id))
                                            .ToListAsync();

                    // Validate: Khong duoc duplicate dong trong Product Item
                    // Neu so luong distinct product id khac so luong product item => co dong trung
                    if (model.MaterialRequestItems.Count !=
                        model.MaterialRequestItems.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<MaterialRequestViewModel>
                                    ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: giá nhập và số lượng không được nhỏ hơn 0
                    if (model.MaterialRequestItems.Any(x => x.RequestQuantity < 0))
                        return Helper.CreateBadRequestResponse<MaterialRequestViewModel>
                                    ("Danh sách sản phẩm không được có số lượng nhỏ hơn 0.");

                    foreach (var item in model.MaterialRequestItems)
                    {
                        var product = allConstructionItemProducts
                                            .Where(x => x.Id == item.ProductId)
                                            .FirstOrDefault();
                        if (product == null)
                            return Helper.CreateBadRequestResponse<MaterialRequestViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");
                    }
                }
                #endregion

                #region Fill Item's Line No

                if (model.MaterialRequestItems != null && model.MaterialRequestItems.Count > 0)
                {
                    model.MaterialRequestItems = model.MaterialRequestItems.OrderBy(x => x.LineNo).ToList();
                    for (int i = 0; i < model.MaterialRequestItems.Count; i++)
                        model.MaterialRequestItems[i].LineNo = i + 1;
                }
                #endregion
                
                // Remove Old Item -> Re-add
                _dbContext.RemoveRange(entity.MaterialRequestItems);
                entity.MaterialRequestItems = new List<sm_MaterialRequestItem>();

                foreach (var modelItem in model.MaterialRequestItems)
                {
                    var item = _mapper.Map<sm_MaterialRequestItem>(modelItem);
                    entity.MaterialRequestItems.Add(item);
                    
                    // Assign MaterialRequestId
                    item.MaterialRequestId = entity.Id;
                    
                    var product = allConstructionItemProducts.FirstOrDefault(x => x.Id == modelItem.ProductId);

                    if (product != null)
                    {
                        item.MaterialRequestId = entity.Id;
                        item.ConstructionId = entity.ConstructionId;
                        item.IsApprove = false;
                        item.Code = product.Code;
                        item.Name = product.Name;
                        item.Unit = product.Unit;
                        item.PlannedQuantity = _dbContext.sm_ConstructionItems
                            .FirstOrDefault(x => x.ConstructionId == entity.ConstructionId && x.ProductId == modelItem.ProductId)
                            .PlannedQuantity;
                    }
                    
                    // var productEntity = await _dbContext.sm_ConstructionItems
                    //     .FirstOrDefaultAsync(x => x.ConstructionId == entity.ConstructionId && x.ProductId == modelItem.ProductId);
                    //
                    // if (productEntity != null)
                    // {
                    //     productEntity.PlannedQuantity -= _dbContext.sm_MaterialRequestItem
                    //         .Where(x => x.ProductId == modelItem.ProductId 
                    //                     && (x.sm_MaterialRequest.StatusCode ==
                    //                         MaterialRequestConstants.StatusCode.APPROVE
                    //                         || x.sm_MaterialRequest.StatusCode ==
                    //                         MaterialRequestConstants.StatusCode.COMPLETED) && x.ConstructionId == entity.ConstructionId)
                    //         .Sum(x => x.RequestQuantity);
                    //     _dbContext.sm_ConstructionItems.Update(productEntity);
                    // }
                }
                
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;
                entity.LastModifiedOnDate = DateTime.Now;
                
                entity.Note =  model.Note;
                entity.Content =  model.Content;
                entity.DateProcess = model.DateProcess;
                entity.Priority = model.Priority;
                
                // B3. Thêm lại item sau khi tính toán
                #region
                foreach (var item in entity.MaterialRequestItems)
                {
                    _dbContext.sm_MaterialRequestItem.Add(item);
                }
                #endregion
                
                #region Cập nhật lịch sử xử lý
                if (entity.ListHistoryProcess == null)
                {
                    allHistoryProcess.Add(new jsonb_HistoryProcess()
                    {
                        UserName =  currentUser.FullName,
                        Description = "đã chỉnh sửa yêu cầu vật tư",
                        CreatedOnDate =  DateTime.Now,
                    });
                    
                    entity.ListHistoryProcess = allHistoryProcess;
                }
                else
                {
                    if (entity.ListHistoryProcess != null && entity.ListHistoryProcess.Count > 0)
                    {
                        foreach (var item in entity.ListHistoryProcess)
                        {
                            allHistoryProcess.Add(item);
                        }

                        var newHistoryProcess = new jsonb_HistoryProcess()
                        {
                            UserName =  currentUser.FullName,
                            Description = "đã chỉnh sửa yêu cầu vật tư",
                            CreatedOnDate =  DateTime.Now,
                        };

                        allHistoryProcess.Add(newHistoryProcess);
                        entity.ListHistoryProcess = allHistoryProcess;
                    }
                }
                #endregion
                
                _dbContext.sm_MaterialRequest.Update(entity);
                await _dbContext.SaveChangesAsync();
                
                return Helper.CreateSuccessResponse<MaterialRequestViewModel>(_mapper.Map<MaterialRequestViewModel>(entity), "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return Helper.CreateExceptionResponse<MaterialRequestViewModel>(ex);
            }
        }

        public async Task<Response<MaterialRequestViewModel>> ApproveMaterialRequest(Guid id, RequestUser currentUser)
        {
            try
            {
                List<jsonb_HistoryProcess> allHistoryProcess = new List<jsonb_HistoryProcess>();
                
                var entity =  await _dbContext.sm_MaterialRequest
                    .Include(x => x.MaterialRequestItems)
                    .ThenInclude(x => x.sm_Product)
                    .Include(x => x.Construction)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return Helper.CreateBadRequestResponse<MaterialRequestViewModel>("Không tìm thấy bản ghi");
                }

                entity.StatusCode = MaterialRequestConstants.StatusCode.APPROVE;
                entity.StatusName = MaterialRequestConstants.FetchStatus(MaterialRequestConstants.StatusCode.APPROVE).Name;

                if (entity.StatusCode == MaterialRequestConstants.StatusCode.APPROVE)
                {
                    #region Cập nhật lịch sử xử lý
                    if (entity.ListHistoryProcess == null)
                    {
                        allHistoryProcess.Add(new jsonb_HistoryProcess()
                        {
                            UserName =  currentUser.FullName,
                            Description = "đã duyệt yêu cầu vật tư",
                            CreatedOnDate =  DateTime.Now,
                        });
                    
                        entity.ListHistoryProcess = allHistoryProcess;
                    }
                    else
                    {
                        if (entity.ListHistoryProcess != null && entity.ListHistoryProcess.Count > 0)
                        {
                            foreach (var item in entity.ListHistoryProcess)
                            {
                                allHistoryProcess.Add(item);
                            }

                            var newHistoryProcess = new jsonb_HistoryProcess()
                            {
                                UserName =  currentUser.FullName,
                                Description = "đã duyệt yêu cầu vật tư",
                                CreatedOnDate =  DateTime.Now,
                            };

                            allHistoryProcess.Add(newHistoryProcess);
                            entity.ListHistoryProcess = allHistoryProcess;
                        }
                    }
                    #endregion
                }
                
                #region Duyệt yêu cầu vật tư sẽ cập nhật số lượng kế hoạch của vật tư đấy
                foreach (var item in entity.MaterialRequestItems)
                {
                    var productEntity = await _dbContext.sm_ConstructionItems
                        .FirstOrDefaultAsync(x => x.ConstructionId == entity.ConstructionId && x.ProductId == item.ProductId);
                    
                    var materialRequestEntity = await _dbContext.sm_MaterialRequestItem
                        .FirstOrDefaultAsync(x => x.ConstructionId == entity.ConstructionId 
                                                  && x.ProductId == item.ProductId 
                                                  && x.MaterialRequestId == item.MaterialRequestId);

                    if (materialRequestEntity != null)
                    {
                        // Đánh dấu những vật tư đã được duyệt
                        materialRequestEntity.IsApprove = true;
                        // Save change
                        _dbContext.sm_MaterialRequestItem.Update(materialRequestEntity);
                        await _dbContext.SaveChangesAsync();
                    }
                    
                    
                    if (productEntity != null)
                    {
                        productEntity.PlannedQuantity -= item.RequestQuantity;
                        // Save change
                        _dbContext.sm_ConstructionItems.Update(productEntity);
                        await _dbContext.SaveChangesAsync();
                    }
                    
                }
                #endregion
                
                _dbContext.sm_MaterialRequest.Update(entity);
                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động duyệt yêu cầu vật tư vào bảng sm_ConstructionActivityLog
                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã duyệt yêu cầu vật tư",
                        CodeLinkDescription = $"{entity.Code} - {entity.Content}",
                        OrderId =  entity.Id,
                        ConstructionId = entity.ConstructionId,
                    }, currentUser);
                    #endregion
                }
                
                return Helper.CreateSuccessResponse<MaterialRequestViewModel>(_mapper.Map<MaterialRequestViewModel>(entity), "Duyệt yêu cầu vật tư thành công");
            }
            catch (Exception ex)
            {
                Log.Error("123", ex);
                return Helper.CreateExceptionResponse<MaterialRequestViewModel>(ex);
            }
        }
        
        public async Task<Response<MaterialRequestViewModel>> RejectApproveMaterialRequest(Guid id, MaterialRejectReason model, RequestUser currentUser)
        {
            try
            {
                List<jsonb_HistoryProcess> allHistoryProcess = new List<jsonb_HistoryProcess>();
                
                var entity =  await _dbContext.sm_MaterialRequest
                    .Include(x => x.Construction)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return Helper.CreateBadRequestResponse<MaterialRequestViewModel>("Không tìm thấy bản ghi");
                }

                entity.StatusCode = MaterialRequestConstants.StatusCode.REJECT;
                entity.StatusName = MaterialRequestConstants.FetchStatus(MaterialRequestConstants.StatusCode.REJECT).Name;

                if (entity.StatusCode == MaterialRequestConstants.StatusCode.REJECT)
                {
                    #region Cập nhật lịch sử xử lý
                    if (entity.ListHistoryProcess == null)
                    {
                        allHistoryProcess.Add(new jsonb_HistoryProcess()
                        {
                            UserName =  currentUser.FullName,
                            Description = "đã từ chối duyệt yêu cầu vật tư",
                            SubDescription = model.Reason,
                            CreatedOnDate =  DateTime.Now,
                        });
                    
                        entity.ListHistoryProcess = allHistoryProcess;
                    }
                    else
                    {
                        if (entity.ListHistoryProcess != null && entity.ListHistoryProcess.Count > 0)
                        {
                            foreach (var item in entity.ListHistoryProcess)
                            {
                                allHistoryProcess.Add(item);
                            }

                            var newHistoryProcess = new jsonb_HistoryProcess()
                            {
                                UserName =  currentUser.FullName,
                                Description = "đã từ chối duyệt yêu cầu vật tư",
                                SubDescription = model.Reason,
                                CreatedOnDate =  DateTime.Now,
                            };

                            allHistoryProcess.Add(newHistoryProcess);
                            entity.ListHistoryProcess = allHistoryProcess;
                        }
                    }
                    #endregion
                }
                
                _dbContext.sm_MaterialRequest.Update(entity);
                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động từ chối duyệt vật tư vào bảng sm_ConstructionActivityLog
                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã từ chối duyệt yêu cầu vật tư",
                        CodeLinkDescription = $"{entity.Code} - {entity.Content}",
                        OrderId =  entity.Id,
                        ConstructionId = entity.ConstructionId,
                    }, currentUser);
                    #endregion
                }
                
                return Helper.CreateSuccessResponse<MaterialRequestViewModel>(_mapper.Map<MaterialRequestViewModel>(entity), "Từ chối duyệt yêu cầu vật tư thành công");
            }
            catch (Exception ex)
            {
                Log.Error("123", ex);
                return Helper.CreateExceptionResponse<MaterialRequestViewModel>(ex);
            }
        }
        
        public async Task<Response<MaterialRequestViewModel>> RequestApproveMaterialRequest(Guid id, RequestUser currentUser)
        {
            try
            {
                List<jsonb_HistoryProcess> allHistoryProcess = new List<jsonb_HistoryProcess>();
                
                var entity =  await _dbContext.sm_MaterialRequest
                    .Include(x => x.Construction)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return Helper.CreateBadRequestResponse<MaterialRequestViewModel>("Không tìm thấy bản ghi");
                }

                entity.StatusCode = MaterialRequestConstants.StatusCode.PENDING_APPROVE;
                entity.StatusName = MaterialRequestConstants.FetchStatus(MaterialRequestConstants.StatusCode.PENDING_APPROVE).Name;

                if (entity.StatusCode == MaterialRequestConstants.StatusCode.PENDING_APPROVE)
                {
                    #region Cập nhật lịch sử xử lý
                    if (entity.ListHistoryProcess == null)
                    {
                        allHistoryProcess.Add(new jsonb_HistoryProcess()
                        {
                            UserName =  currentUser.FullName,
                            Description = "đã gửi duyệt yêu cầu vật tư",
                            CreatedOnDate =  DateTime.Now,
                        });
                    
                        entity.ListHistoryProcess = allHistoryProcess;
                    }
                    else
                    {
                        if (entity.ListHistoryProcess != null && entity.ListHistoryProcess.Count > 0)
                        {
                            foreach (var item in entity.ListHistoryProcess)
                            {
                                allHistoryProcess.Add(item);
                            }

                            var newHistoryProcess = new jsonb_HistoryProcess()
                            {
                                UserName =  currentUser.FullName,
                                Description = "đã gửi duyệt yêu cầu vật tư",
                                CreatedOnDate =  DateTime.Now,
                            };

                            allHistoryProcess.Add(newHistoryProcess);
                            entity.ListHistoryProcess = allHistoryProcess;
                        }
                    }
                    #endregion
                }
                
                _dbContext.sm_MaterialRequest.Update(entity);
                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động gửi duyệt yêu cầu vật tư vào bảng sm_ConstructionActivityLog
                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã gửi duyệt yêu cầu vật tư",
                        CodeLinkDescription = $"{entity.Code} - {entity.Content}",
                        OrderId =  entity.Id,
                        ConstructionId = entity.ConstructionId,
                    }, currentUser);
                    #endregion
                }
                
                return Helper.CreateSuccessResponse<MaterialRequestViewModel>(_mapper.Map<MaterialRequestViewModel>(entity), "Gửi duyệt yêu cầu vật tư thành công");
            }
            catch (Exception ex)
            {
                Log.Error("123", ex);
                return Helper.CreateExceptionResponse<MaterialRequestViewModel>(ex);
            }
        }

        /// <summary>
        /// Lấy chi tiết yêu cầu vật tư
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<MaterialRequestViewModel>> GetById(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await  _dbContext.sm_MaterialRequest
                    .Include(x => x.MaterialRequestItems)
                    .ThenInclude(x => x.sm_Product)
                    .Include(x => x.Construction)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return Helper.CreateBadRequestResponse<MaterialRequestViewModel>("Không tìm thấy bản ghi");
                }
                
                return Helper.CreateSuccessResponse<MaterialRequestViewModel>(_mapper.Map<MaterialRequestViewModel>(entity));
            }
            catch (Exception ex)
            {
                return Helper.CreateExceptionResponse<MaterialRequestViewModel>(ex);
            }
        }

        /// <summary>
        /// Xoá yêu cầu vật tư
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<MaterialRequestViewModel>> Delete(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await  _dbContext.sm_MaterialRequest
                    .Include(x => x.Construction)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return Helper.CreateBadRequestResponse<MaterialRequestViewModel>("Không tìm thấy bản ghi");
                }
                
                _dbContext.sm_MaterialRequest.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse<MaterialRequestViewModel>(null,"Xoá thành công");
            }
            catch (Exception ex)
            {
                return Helper.CreateExceptionResponse<MaterialRequestViewModel>(ex);
            }
        }

        /// <summary>
        /// Danh sách yêu cầu vật tư
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Response<Pagination<MaterialRequestViewModel>>> GetPage(MaterialRequestQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                
                var queryResult = _dbContext.sm_MaterialRequest.AsNoTracking()
                    .Include(x => x.MaterialRequestItems)
                    .ThenInclude(x => x.sm_Product)
                    .Include(x => x.Construction)
                    .Where(predicate);
                
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<MaterialRequestViewModel>>(data);
                
                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                return Helper.CreateExceptionResponse<Pagination<MaterialRequestViewModel>>(ex);
            }
        }
        
        private Expression<Func<sm_MaterialRequest, bool>> BuildQuery(MaterialRequestQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_MaterialRequest>(true);
            
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate = predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower())
                                               || s.CreatedByUserName.ToLower().Contains(query.FullTextSearch.ToLower())
                                               || s.Content.ToLower().Contains(query.FullTextSearch.ToLower())
                );

            if (query.ConstructionId.HasValue && query.ConstructionId.Value != Guid.Empty)
                predicate = predicate.And(s => s.ConstructionId == query.ConstructionId);

            if (query.DueDate != null)
                predicate = predicate.And(s => s.DateProcess.Date == query.DueDate.Value.Date);

            if (query.CreatedOnDate != null)
                predicate = predicate.And(s => s.CreatedOnDate.Date == query.CreatedOnDate.Value.Date);

            if (!string.IsNullOrEmpty(query.StatusCode))
                predicate = predicate.And(s => s.StatusCode == query.StatusCode);

            if (!string.IsNullOrEmpty(query.PriorityLevelCode))
                predicate = predicate.And(s => s.Priority == query.PriorityLevelCode);
            
            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date <= query.DateRange[1].Value.Date);
            }
            
            return predicate;
        }
    }
}

