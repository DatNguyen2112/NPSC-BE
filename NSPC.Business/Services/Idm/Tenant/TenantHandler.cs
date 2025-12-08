using AutoMapper;
using FirebaseAdmin.Auth.Multitenancy;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Driver.Linq;
using NSPC.Business.Services;
using NSPC.Business.Services.StockTransaction;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.CauHinhNhanSu;
using NSPC.Data.Entity;
using Serilog;
using SharpCompress.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static NSPC.Common.Helper;

namespace NSPC.Business
{
    public class TenantHandler : ITenantHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IStockTransactionHandler _stockTransactionHandler;
        private readonly string _staticsFolder;
        private readonly IUserHandler _userHandler;
        private readonly IAttachmentHandler _attachmentHandler;

        public TenantHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IStockTransactionHandler stockTransactionHandler, IUserHandler userHandler, 
            IAttachmentHandler attachmentHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _stockTransactionHandler = stockTransactionHandler;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
            _userHandler = userHandler;
            _attachmentHandler = attachmentHandler;
        }
        public async Task<Response<TenantModel>> Create(TenantCreateModel model, Helper.RequestUser requestUser)
        {
            try
            {
                if (_dbContext.Idm_Tenants.Any(x => x.Email == model.Email))
                    return Helper.CreateBadRequestResponse<TenantModel>("Email đã được đăng ký trong hệ thống. Vui lòng nhập thông tin khác");
                if (_dbContext.Idm_Tenants.Any(x => x.PhoneNumber == model.PhoneNumber))
                    return Helper.CreateBadRequestResponse<TenantModel>("Số điện thoại đã được đăng ký trong hệ thống. Vui lòng nhập thông tin khác");
                if (_dbContext.Idm_Tenants.Any(x => x.SubDomain == model.SubDomain))
                    return Helper.CreateBadRequestResponse<TenantModel>("Tên miền danh đã được đăng ký trong hệ thống. Vui lòng nhập thông tin khác");
                var entity = _mapper.Map<Idm_Tenant>(model);
                entity.Id = Guid.NewGuid();
                entity.StatusCode = TenantStatusConstant.ACTIVE;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.Add(entity);
                var status = await _dbContext.SaveChangesAsync();
                if (status > 0)
                {
                    await processAttachment(entity.Id, model.Attachments);
                    await CreateRole(entity);
                    await CreateNavigationDefault(entity);
                    await CreateNavigationMapRole(entity);
                    await CreateRightDefault(entity);
                    await CreateUserDefault(entity, model);
                    await CreateCodeTypeDefault(entity);
                }
                return Helper.CreateSuccessResponse<TenantModel>(_mapper.Map<TenantModel>(entity), "Đăng ký thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Model: {@Param}", model);
                return Helper.CreateExceptionResponse<TenantModel>(ex);
            }
        }

        public async Task<Response> Delete(Guid id, Helper.RequestUser requestUser)
        {
            try
            {
                var entity = await _dbContext.Idm_Tenants.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateBadRequestResponse<TenantModel>("Không có tenant này");
                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse("Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<TenantModel>(ex);
            }
        }

        public async Task<Response<TenantModel>> GetById(Guid id, Helper.RequestUser requestUser)
        {
            try
            {
                var entity = await _dbContext.Idm_Tenants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateBadRequestResponse<TenantModel>("Không có tenant này");
                return Helper.CreateSuccessResponse<TenantModel>(_mapper.Map<TenantModel>(entity));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<TenantModel>(ex);
            }
        }

        public async Task<Response<Pagination<TenantModel>>> GetPageAsync(TenantQueryModel query, Helper.RequestUser requestUser)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.Idm_Tenants.AsNoTracking()
                    .Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<TenantModel>>(data);

                return Helper.CreateSuccessResponse<Pagination<TenantModel>>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<TenantModel>>(ex);
            }
        }
        private Expression<Func<Idm_Tenant, bool>> BuildQuery(TenantQueryModel query)
        {
            var predicate = PredicateBuilder.New<Idm_Tenant>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.Name.ToLower().Contains(query.FullTextSearch.ToLower()));


            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date <= query.DateRange[1].Value.Date);
            }

            return predicate;
        }
        public async Task<Response<TenantModel>> RegisterTenant(TenantCreateModel model, RequestUser requestUser)
        {
            try
            {
                if (_dbContext.Idm_Tenants.Any(x => x.Email == model.Email))
                    return Helper.CreateBadRequestResponse<TenantModel>("Email đã được đăng ký trong hệ thống. Vui lòng nhập thông tin khác");
                if (_dbContext.Idm_Tenants.Any(x => x.PhoneNumber == model.PhoneNumber))
                    return Helper.CreateBadRequestResponse<TenantModel>("Số điện thoại đã được đăng ký trong hệ thống. Vui lòng nhập thông tin khác");
                if (_dbContext.Idm_Tenants.Any(x => x.SubDomain == model.SubDomain))
                    return Helper.CreateBadRequestResponse<TenantModel>("Tên miền danh đã được đăng ký trong hệ thống. Vui lòng nhập thông tin khác");
                var entity = _mapper.Map<Idm_Tenant>(model);
                entity.Id = Guid.NewGuid();
                entity.StatusCode = TenantStatusConstant.ACTIVE;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.Add(entity);
                var status = await _dbContext.SaveChangesAsync();
                if (status > 0)
                {
                    await processAttachment(entity.Id, model.Attachments);
                    await CreateRole(entity);
                    await CreateNavigationDefault(entity);
                    await CreateNavigationMapRole(entity);
                    await CreateRightDefault(entity);
                    await CreateUserDefault(entity, model);
                    await CreateCodeTypeDefault(entity);
                }
                return Helper.CreateSuccessResponse<TenantModel>(_mapper.Map<TenantModel>(entity), "Đăng ký thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Model: {@Param}", model);
                return Helper.CreateExceptionResponse<TenantModel>(ex);
            }
        }

        public async Task<Response<TenantModel>> Update(Guid id, TenantUpdateModel model, Helper.RequestUser requestUser)
        {
            try
            {
                var entity = await _dbContext.Idm_Tenants.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateBadRequestResponse<TenantModel>("Không có tenant này");
                if (_dbContext.Idm_Tenants.Any(x => x.Email == model.Email && x.Id != entity.Id))
                    return Helper.CreateBadRequestResponse<TenantModel>("Email đã được đăng ký trong hệ thống. Vui lòng nhập thông tin khác");
                if (_dbContext.Idm_Tenants.Any(x => x.PhoneNumber == model.PhoneNumber && x.Id != entity.Id))
                    return Helper.CreateBadRequestResponse<TenantModel>("Số điện thoại đã được đăng ký trong hệ thống. Vui lòng nhập thông tin khác");
                _mapper.Map(model, entity);
                await _dbContext.SaveChangesAsync();
                await processAttachment(entity.Id, model.Attachments);
                return Helper.CreateSuccessResponse<TenantModel>(_mapper.Map<TenantModel>(entity), "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Model: {@Param}", model);
                return Helper.CreateExceptionResponse<TenantModel>(ex);
            }
        }

        public async Task<Response> CheckDomainExists(string domain)
        {
            try
            {
                var entity = await _dbContext.Idm_Tenants.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(x => x.SubDomain == domain);
                if (entity == null)
                    return Helper.CreateForbiddenResponse();
                return Helper.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateForbiddenResponse();
            }
        }
        private async Task processAttachment(Guid id, List<jsonb_Attachment> attachments)
        {
            try
            {
                var attachmentListId = attachments.Select(x => x.Id).ToList();
                // Process attachments
                if (attachmentListId.Count > 0)
                {
                    var newEntity =
                        await _dbContext.Idm_Tenants.Where(x => x.Id == id).FirstOrDefaultAsync();
                    var allAttachments = await _dbContext.erp_Attachment.Where(x => attachmentListId.Contains(x.Id))
                        .ToListAsync();

                    foreach (var att in allAttachments)
                    {
                        // UpdatePaid entity
                        att.EntityId = newEntity.Id;
                        att.EntityType = attachments.Where(x => x.Id == att.Id).FirstOrDefault()?.DocType;
                        att.Description = attachments.Where(x => x.Id == att.Id).FirstOrDefault()?.Description;

                        // Move files to new folder
                        var moveFileResult = _attachmentHandler.MoveEntityAttachment(att.DocType, att.EntityType,
                            newEntity.Id, att.FilePath, newEntity.CreatedOnDate);
                        if (moveFileResult.IsSuccess)
                            att.FilePath = moveFileResult.Data;

                        if(att.DocType == AttachmentDocTypeConstants.Logo)
                        {
                            var fullPath = Path.Combine(_staticsFolder, att.FilePath);
                            var logoPath = string.Format("{0}/{1}", "tenant", "logo");
                            if (!Directory.Exists(Path.Combine(_staticsFolder, logoPath)))
                                Directory.CreateDirectory(Path.Combine(_staticsFolder, logoPath));
                            var newFileName = string.Format("{0}", $"{newEntity.SubDomain}-logo.png");
                            var newFullPath = Path.Combine(_staticsFolder, logoPath, newFileName);
                            using(Image img = Image.Load(fullPath))
                            {
                                img.Save(newFullPath, new PngEncoder());
                            }
                        }
                        
                    }

                    if (allAttachments != null && allAttachments.Count() > 0)
                        newEntity.Attachments = allAttachments.Select(x => new jsonb_Attachment
                        {
                            Description = x.Description,
                            DocType = x.DocType,
                            FilePath = x.FilePath,
                            //Name = x.Name,
                            Id = x.Id
                        }).ToList();
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        public async Task<Response<TenantModel>> GetByDomain(string domain)
        {
            try
            {
                var entity = await _dbContext.Idm_Tenants.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(x => x.SubDomain == domain);
                if (entity == null)
                    return Helper.CreateBadRequestResponse<TenantModel>("Không có tenant này");
                return Helper.CreateSuccessResponse<TenantModel>(_mapper.Map<TenantModel>(entity));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<TenantModel>(ex);
            }
        }

        #region Init default
        private async Task CreateRole(Idm_Tenant entity)
        {
            try
            {
                var adminRole = new IdmRole()
                {
                    Code = RoleConstants.AdminRoleCode,
                    Id = Guid.NewGuid(),
                    TenantId = entity.Id,
                    CreatedOnDate = DateTime.Now,
                    Name = "Administrator",
                    Level = 10,
                    Description = "Full quyền toàn bộ hệ thống"
                };
                _dbContext.Add(adminRole);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
            }
        }
        private async Task CreateNavigationDefault(Idm_Tenant entity)
        {
            try
            {
                var listNavDefault = await _dbContext.BsdNavigation.IgnoreQueryFilters().Where(x => x.IsDefault).ToListAsync();
                var listParentNav = listNavDefault.Where(x => !x.ParentId.HasValue).ToList();
                if (listParentNav.Count == 0)
                    return;
                foreach (var item in listParentNav)
                {
                    var newTenantNav = new BsdNavigation();
                    newTenantNav.Id = Guid.NewGuid();
                    newTenantNav.ParentId = null;
                    newTenantNav.Name = item.Name;
                    newTenantNav.Code = item.Code;
                    newTenantNav.UrlRewrite = item.UrlRewrite;
                    newTenantNav.IdPath = newTenantNav.Id.ToString() + "/";
                    newTenantNav.Path = item.Path;
                    newTenantNav.SubUrl = item.SubUrl;
                    newTenantNav.IconClass = item.IconClass;
                    newTenantNav.Status = item.Status;
                    newTenantNav.Order = item.Order;
                    newTenantNav.HasChild = item.HasChild;
                    newTenantNav.Level = item.Level;
                    newTenantNav.Type = item.Type;
                    newTenantNav.QueryParams = item.QueryParams;
                    newTenantNav.TenantId = entity.Id;
                    newTenantNav.CreatedOnDate = DateTime.Now;
                    newTenantNav.IsDefault = false;
                    _dbContext.Add(newTenantNav);
                    await CreateChildNavDefault(entity, listNavDefault, item, newTenantNav);
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
            }
        }
        private async Task CreateChildNavDefault(Idm_Tenant entity, List<BsdNavigation> listNav, BsdNavigation defaultNav, BsdNavigation parentNav)
        {
            try
            {
                var listChild = listNav.Where(x => x.ParentId == defaultNav.Id).ToList();
                if (listChild.Count > 0)
                    foreach (var item in listChild)
                    {
                        var newTenantNav = new BsdNavigation();
                        newTenantNav.Id = Guid.NewGuid();
                        newTenantNav.ParentId = parentNav.Id;
                        newTenantNav.Name = item.Name;
                        newTenantNav.Code = item.Code;
                        newTenantNav.UrlRewrite = item.UrlRewrite;
                        newTenantNav.IdPath = parentNav.IdPath + newTenantNav.Id.ToString() + "/";
                        newTenantNav.Path = item.Path;
                        newTenantNav.SubUrl = item.SubUrl;
                        newTenantNav.IconClass = item.IconClass;
                        newTenantNav.Status = item.Status;
                        newTenantNav.Order = item.Order;
                        newTenantNav.HasChild = item.HasChild;
                        newTenantNav.Level = item.Level;
                        newTenantNav.Type = item.Type;
                        newTenantNav.QueryParams = item.QueryParams;
                        newTenantNav.TenantId = entity.Id;
                        newTenantNav.CreatedOnDate = DateTime.Now;
                        newTenantNav.IsDefault = false;
                        _dbContext.Add(newTenantNav);
                        await CreateChildNavDefault(entity, listNav, item, newTenantNav);
                    }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
            }
        }
        private async Task CreateNavigationMapRole(Idm_Tenant entity)
        {
            try
            {
                var listNav = await _dbContext.BsdNavigation.IgnoreQueryFilters().Where(x => x.TenantId == entity.Id).OrderByDescending(x => x.ParentId).ToListAsync();
                var adminRole = await _dbContext.IdmRole.IgnoreQueryFilters().Where(x => x.TenantId == entity.Id && RoleConstants.AdminRoleCode == x.Code).FirstOrDefaultAsync();
                if (adminRole == null)
                    return;
                foreach (var item in listNav)
                {
                    var newMapRole = new BsdNavigationMapRole()
                    {
                        NavigationId = item.Id,
                        FromSubNavigation = item.ParentId,
                        RoleId = adminRole.Id,
                        TenantId = entity.Id,
                    };
                    _dbContext.Add(newMapRole);
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
            }
        }
        private async Task CreateUserDefault(Idm_Tenant entity, TenantCreateModel model)
        {
            try
            {
                var newUser = new idm_User();
                newUser.Id = Guid.NewGuid();
                newUser.UserName = entity.Email;
                newUser.Email = entity.Email;
                newUser.PhoneNumber = entity.PhoneNumber;
                newUser.IsActive = true;
                newUser.IsEmailVerified = true;
                newUser.IsLockedOut = false;
                newUser.ActiveDate = DateTime.Now;
                newUser.Gender = "MALE";
                newUser.TenantId = entity.Id;
                newUser.Name = model.Name;
                newUser.Password = model.Password;
                if (newUser.UserName == null)
                {
                    newUser.UserName = newUser.Email;
                }

                //Cập nhật pass
                if (!string.IsNullOrEmpty(newUser.Password))
                {
                    newUser.PasswordSalt = AccountHelper.CreatePasswordSalt();
                    newUser.Password = AccountHelper.HashPassword(newUser.Password, newUser.PasswordSalt);
                    newUser.PlainTextPwd = model.Password;
                }
                newUser.RoleListCode = new List<string>() { RoleConstants.AdminRoleCode };
                var newCauHinhNhanSu = new mk_CauHinhNhanSu
                {
                    IdUser = newUser.Id,
                };

                _dbContext.IdmUser.Add(newUser);
                _dbContext.mk_CauHinhNhanSu.Add(newCauHinhNhanSu);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
            }
        }
        private async Task CreateRightDefault(Idm_Tenant entity)
        {
            try
            {
                var listDefaultRight = await _dbContext.IdmRight.IgnoreQueryFilters().Where(x => x.IsDefault).ToListAsync();
                var roleDefault = await _dbContext.IdmRole.IgnoreQueryFilters().Where(x => x.TenantId == entity.Id && RoleConstants.AdminRoleCode == x.Code).FirstOrDefaultAsync();
                foreach(var item in listDefaultRight)
                {
                    var newRight = new IdmRight()
                    {
                        Id = Guid.NewGuid(),
                        Name = item.Name,
                        Code = item.Code,
                        Description = item.Description,
                        GroupCode = item.GroupCode,
                        GroupName = item.GroupName,
                        TenantId = entity.Id,
                        IsDefault = false
                    };
                    var newRightMapRole = new IdmRightMapRole()
                    {
                        Id = Guid.NewGuid(),
                        RightId = newRight.Id,
                        RoleId = roleDefault.Id,
                        TenantId = entity.Id
                    };
                    _dbContext.Add(newRight);
                    _dbContext.Add(newRightMapRole);
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
            }
        }
        private async Task CreateCodeTypeDefault(Idm_Tenant entity)
        {
            try
            {
                var listType = new List<string>() 
                { 
                    CodeTypeConstants.Don_Vi_Tinh,
                    CodeTypeConstants.Purpose_Receipt,
                    CodeTypeConstants.Expenditure_Purpose,
                    CodeTypeConstants.Payment_Method,
                    CodeTypeConstants.PaymentGroup,
                    CodeTypeConstants.ActionCode,
                    CodeTypeConstants.ReturnedReasonCode,
                    CodeTypeConstants.InventoryImportType,
                    CodeTypeConstants.InventoryExportType,
                    CodeTypeConstants.CustomerGroup,
                    CodeTypeConstants.SupplierGroup,
                    CodeTypeConstants.VATList,
                    CodeTypeConstants.InventoryCheckNoteReason,
                    CodeTypeConstants.CustomerType,
                    CodeTypeConstants.CustomerSource,
                    CodeTypeConstants.AssetType,
                    CodeTypeConstants.AssetGroup,
                };
                var listCodeType = await _dbContext.sm_CodeType.IgnoreQueryFilters().AsNoTracking().Where(x => listType.Contains(x.Type) && !x.TenantId.HasValue).ToListAsync();
                foreach(var item in listCodeType)
                {
                    var newCode = _mapper.Map<sm_CodeType>(item);
                    newCode.Id = Guid.NewGuid();
                    newCode.TenantId = entity.Id;
                    _dbContext.Add(newCode);
                }
                var status = await _dbContext.SaveChangesAsync();
                if(status > 0)
                {
                    CodeTypeCollection.Instance.FillDataByTenant(entity.Id);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
            }
        }
        #endregion Init default
    }
}
