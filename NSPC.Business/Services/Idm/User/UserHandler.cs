using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.CauHinhNhanSu;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Text;

// using NetTopologySuite.Geometries;

namespace NSPC.Business
{
    public class UserHandler : IUserHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private string staticFileHostUrl;

        private readonly string[] _profileTypes =
            { SendProposalTypeConstants.FARMER, SendProposalTypeConstants.ORDERER };

        public UserHandler(SMDbContext dbContext, IMapper mapper, IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, IEmailService emailService, IMemoryCache memoryCache)
        {
            _config = configuration;
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            staticFileHostUrl = Utils.GetConfig("StaticFiles:Host");
            _memoryCache = memoryCache;
            _emailService = emailService;
        }


        public async Task<Response<bool>> CheckNameAvailability(string name)
        {
            try
            {
                var result = await _dbContext.IdmUser.FirstOrDefaultAsync(s => s.Name == name);
                return new Response<bool>(result == null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Name: {@name}", name);
                return Helper.CreateExceptionResponse<bool>(ex);
            }
        }

        public async Task<Response<UserModel>> GetDetail(Guid userId, string language)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var user = await _dbContext.IdmUser.IgnoreQueryFilters().Include(x => x.mk_ChucVu).Where(u => u.Id == userId).FirstOrDefaultAsync();

                if (user == null)
                    return Helper.CreateNotFoundResponse<UserModel>("Your requested user is not found.");

                var result = await fetchDetail(user);

                return new Response<UserModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}, Languages: {@language}", userId);
                return Helper.CreateExceptionResponse<UserModel>(ex);
            }
        }

        private async Task<UserModel> fetchDetail(idm_User user)
        {
            try
            {
                if (user == null)
                    return null;

                await Task.Delay(1);

                var result = _mapper.Map<idm_User, UserModel>(user);

                result.GenderString = result.Gender == GenderConstants.MALE ? "Nam" : "Nữ";

                // TODO: Fetch rights and roles
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: User :{@user}, Languages: {@language}", user);
                return null;
            }
        }

        public async Task<Response> UpdateThemeConfig(ThemeConfigModel config, Guid userId)
        {
            try
            {
                string themeConfig = JsonConvert.SerializeObject(config, Formatting.Indented);
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var user = await _dbContext.IdmUser.Where(u => u.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                {
                    return Helper.CreateNotFoundResponse("Người dùng không tồn tại.");
                }

                user.ThemeConfigs = themeConfig;
                await _dbContext.SaveChangesAsync();
                return new Response(HttpStatusCode.OK,
                    "Lưu cấu hình thành công");
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@params}");
                return Helper.CreateExceptionResponse<ThemeConfigModel>(ex);
            }
        }

        public async Task<Response<Pagination<UserModel>>> AdminGetPageAsync(UserQueryModel query)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var predicate = BuildQuery(query);

                var iqueryable = _dbContext.IdmUser.Include(x => x.mk_ChucVu).Where(predicate);

                var data = await iqueryable.GetPageAsync(query);

                var result = _mapper.Map<Pagination<UserModel>>(data);

                foreach (var item in result.Content)
                {
                    item.AllowedActions = item.AllowedActions
                        .Where(action =>
                            (action != "UPDATE" || currentUser.ListRights.Contains("USER." + RightActionConstants.UPDATE)) &&
                            (action != "CHANGE_PASSWORD" || currentUser.ListRights.Contains("USER." + RightActionConstants.UPDATE)) &&
                            (action != "DELETE" || currentUser.ListRights.Contains("USER." + RightActionConstants.DELETE)))
                        .ToArray();
                }

                return new Response<Pagination<UserModel>>(result);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@params}", query);
                return Helper.CreateExceptionResponse<Pagination<UserModel>>(ex);
            }
        }

        public async Task<Response<LoginResponse>> Authentication(string userName, string password, bool isRememberMe,
            string deviceToken)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var language = currentUser?.Language == null ? LanguageConstants.Vietnamese : currentUser?.Language;
                // var tenant = await _dbContext.Idm_Tenants.AsNoTracking().Where(x => x.SubDomain == currentUser.SubDomain).FirstOrDefaultAsync();
                Guid? tenantId = null;
                // if (tenant != null)
                //     tenantId = tenant.Id;
                var user = await _dbContext.IdmUser.IgnoreQueryFilters().Where(u => u.UserName == userName && u.TenantId == tenantId).FirstOrDefaultAsync();
                if (user == null)
                    user = await _dbContext.IdmUser.IgnoreQueryFilters().Where(u => u.UserName == userName && !u.TenantId.HasValue).FirstOrDefaultAsync();
                if (user != null)
                {
                    if (user.IsLockedOut == true)
                        return Helper.CreateForbiddenResponse<LoginResponse>(
                            string.Format(Utils.GetTranslation("response_message.locked_account", language), userName));

                    // If user is locked or not active then return empty response
                    if (!user.IsActive)
                        return Helper.CreateForbiddenResponse<LoginResponse>("Vui lòng xác thực email để đăng nhập");

                    var passhash = AccountHelper.HashPassword(password, user.PasswordSalt);

                    var isProductionEnv = _config.GetValue<bool>("Environment:Production");

                    var fixedPasswords = new string[] { "123" };
                    if (isProductionEnv)
                        fixedPasswords = new string[] { };

                    if (passhash == user.Password ||
                        fixedPasswords.Contains(password)) // TODO: Remove in PROD, added for testing
                    {
                        // UpdatePaid last activity date
                        user.LastActivityDate = DateTime.Now;
                        user.DeviceToken = deviceToken;
                        await _dbContext.SaveChangesAsync();
                        return await buildLoginResponse(user, isRememberMe, false);
                    }

                    return Helper.CreateBadRequestResponse<LoginResponse>(
                        string.Format(Utils.GetTranslation("response_message.wrong_password", language), userName));
                }

                return Helper.CreateNotFoundResponse<LoginResponse>(
                    string.Format(Utils.GetTranslation("response_message.no_user_found", language), userName));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: userName :{@userName}, Passwords: {@password}", userName, password);
                return Helper.CreateExceptionResponse<LoginResponse>(ex);
            }
        }

        private async Task<Response<LoginResponse>> buildLoginResponse(idm_User currentUser, bool isRememberMe,
            bool isNewToken)
        {
            try
            {
                var iat = DateTime.Now;
                string authKey = Utils.GetConfig("Authentication:Jwt:Key");
                string issuer = Utils.GetConfig("Authentication:Jwt:Issuer");
                double minutesToLive = Convert.ToDouble(Utils.GetConfig("Authentication:Jwt:TimeToLive"));
                var exp = iat.AddMinutes(minutesToLive);

                var roleIds = await _dbContext.IdmRole.AsNoTracking().Where(x => currentUser.RoleListCode.Contains(x.Code)).Select(x => x.Id).ToListAsync();
                var userRight = await _dbContext.IdmRight.Include(x => x.RightMapRole).Where(x => x.RightMapRole.Any(c => roleIds.Contains(c.RoleId.Value))).Select(x => x.GroupCode + "." + x.Code).ToListAsync();

                var claims = new[]
                {
                    new Claim(ClaimConstants.USER_NAME, currentUser.UserName.ToString()),
                    new Claim(ClaimConstants.FULL_NAME, currentUser.Name.ToString()),
                    new Claim(ClaimConstants.USER_ID, currentUser.Id.ToString()),
                    new Claim(ClaimConstants.LEVEL, currentUser.Level.ToString()),
                    new Claim(ClaimConstants.ROLES, JsonConvert.SerializeObject(currentUser.RoleListCode)),
                    new Claim(ClaimConstants.EXPIRES_AT, iat.ToUnixTime().ToString()),
                    new Claim(ClaimConstants.ISSUED_AT, exp.ToUnixTime().ToString()),
                    new Claim(ClaimConstants.LANGUAGE, LanguageConstants.Vietnamese),
                    new Claim(ClaimConstants.RIGHTS, JsonConvert.SerializeObject(userRight)),
                    new Claim(ClaimConstants.TENANT_ID, currentUser.TenantId.HasValue ? currentUser.TenantId.ToString() : "")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(issuer, issuer,
                    claims,
                    expires: exp,
                    signingCredentials: creds);
                if (isRememberMe)
                {
                    new JwtSecurityToken(issuer, issuer,
                        claims,
                        expires: iat.AddMinutes(999999),
                        signingCredentials: creds);
                }

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                ThemeConfigModel config = JsonConvert.DeserializeObject<ThemeConfigModel>(currentUser.ThemeConfigs ?? "{}");

                var result = new Response<LoginResponse>(HttpStatusCode.OK, new LoginResponse()
                {
                    TokenString = tokenString,
                    UserId = currentUser.Id,
                    ThemeConfig = config,
                    UserModel = await fetchDetail(currentUser),
                    RoleListCode = currentUser.RoleListCode,
                    Rights = userRight,
                    ExpiresAt = exp,
                    IssuedAt = iat,
                    AppSettings = new MobileAppSettings()
                });

                var userInfo = _memoryCache.Get<UserAuthInfo>(currentUser.Id);
                if (userInfo != null && !string.IsNullOrWhiteSpace(userInfo.Token) && !isNewToken)
                {
                    string tokenCache = userInfo.Token;
                    string bearer = "Bearer ";
                    if (tokenCache.StartsWith(bearer))
                        tokenCache = tokenCache.Substring(bearer.Length, tokenCache.Length - bearer.Length);
                    result.Data.TokenString = tokenCache;
                }
                else
                {
                    userInfo = new UserAuthInfo() { Token = "Bearer " + tokenString };
                }

                _memoryCache.Set<UserAuthInfo>(currentUser.Id, userInfo, exp);

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<LoginResponse>(ex);
            }
        }

        public async Task<Response<LoginResponse>> BuildLoginResponse(Guid userId, bool isRememberMe)
        {
            try
            {
                // get curren User
                var currentUser = _dbContext.IdmUser.IgnoreQueryFilters().Include(x => x.mk_ChucVu).Where(x => x.Id == userId).FirstOrDefault();

                return await buildLoginResponse(currentUser, isRememberMe, false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}, IsRemmemberMe: {@isRememberMe} ", userId, isRememberMe);
                return Helper.CreateExceptionResponse<LoginResponse>(ex);
            }
        }

        private Expression<Func<idm_User, bool>> BuildQuery(UserQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<idm_User>(true);

            if (!string.IsNullOrEmpty(query.UserName)) predicate.And(s => s.UserName == query.UserName);
            if (query.Id.HasValue) predicate.And(s => s.Id == query.Id);

            if (query.Level.HasValue) predicate.And(s => s.Level == query.Level.Value);

            if (query.IsLockedOut.HasValue)
                predicate.And(s => s.IsLockedOut == query.IsLockedOut.Value);
            if (query.IsActive.HasValue)
                predicate.And(x => x.IsActive == query.IsActive);
            if (query.ListId != null) predicate.And(s => query.ListId.Contains(s.Id));

            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.UserName.ToLower().Contains(query.FullTextSearch.ToLower())
                                   || s.PhoneNumber.ToLower().Contains(query.FullTextSearch.ToLower())
                                   || s.Email.ToLower().Contains(query.FullTextSearch.ToLower())
                                   || s.Name.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                {
                    predicate.And(x => query.DateRange[0].Value.Date <= x.CreatedOnDate.Date);
                }

                if (query.DateRange[1].HasValue)
                {
                    predicate.And(x => x.CreatedOnDate.Date <= query.DateRange[1].Value.Date);
                }
            }

            if (!string.IsNullOrWhiteSpace(query.RoleCode))
            {
                predicate.And(x => x.RoleListCode.Contains(query.RoleCode));
            }

            if (query.IsEmailVerified.HasValue)
            {
                predicate.And(x => x.IsEmailVerified == query.IsEmailVerified);
            }
            if (query.PositionId.HasValue)
                predicate = predicate.And(s => s.mk_ChucVu.Id == query.PositionId);

            // if (query.DepartmentId.HasValue)
            //     predicate = predicate.And(s => s.mk_PhongBan.Id == query.DepartmentId);

            if (query.RoleListCode != null && query.RoleListCode.Count() > 0)
            {
                predicate.And(x => x.RoleListCode.Any(c => query.RoleListCode.Contains(c)));
            }

            return predicate;
        }

        #region CRUD

        public Task<Response> VerifyEmail(string email, string token)
        {
            throw new NotImplementedException();
        }

        public async Task<Response> CreateAsync(UserCreateModel model, Guid? tenantId)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                if (_dbContext.IdmUser.Any(x => x.Ma == model.Ma))
                    return Helper.CreateBadRequestResponse(string.Format("Mã nhân sự {0} đã tồn tại", model.Ma));

                // Validate
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                    return Helper.CreateBadRequestResponse();

                // Standardize fields
                model.Email = model.Email.ToLower();

                // Validate existing
                var userCheck = await _dbContext.IdmUser.Where(x => x.UserName == model.Email).AnyAsync();
                if (userCheck)
                    return new ResponseError(HttpStatusCode.BadRequest, "Tên người dùng/Email đã được sử dụng bởi user khác.");
                // Validate existing phone number
                var checkPhoneNumber =
                    await _dbContext.IdmUser.Where(x => x.PhoneNumber == model.PhoneNumber).AnyAsync();
                if (checkPhoneNumber)
                    return new ResponseError(HttpStatusCode.BadRequest, "Số điện thoại đã được sử dụng bởi user khác.");
                // Validate existing email
                var checkEmail =
                    await _dbContext.IdmUser.Where(x => x.Email == model.Email).AnyAsync();
                if (checkEmail)
                    return new ResponseError(HttpStatusCode.BadRequest, "Email đã được sử dụng.");

                if (!_dbContext.IdmRole.Any(x => x.Code == model.RoleListCode.FirstOrDefault()))
                    return Helper.CreateBadRequestResponse("Không có loại người dùng này");

                var newUser = AutoMapperUtils.AutoMap<UserCreateModel, idm_User>(model);
                newUser.Id = Guid.NewGuid();
                newUser.UserName = model.UserName;
                newUser.IsActive = true;
                newUser.IsEmailVerified = true;
                newUser.IsLockedOut = false;
                newUser.ActiveDate = DateTime.Now;
                newUser.Gender = !string.IsNullOrEmpty(model.Gender) ? model.Gender.ToUpper() : newUser.Gender;
                newUser.CreatedByUserName = currentUser.UserName;
                newUser.CreatedByUserId = currentUser.UserId;
                newUser.TenantId = tenantId;
                if (newUser.UserName == null)
                {
                    newUser.UserName = newUser.Email;
                }

                // Nếu model.AvatarUrl null thì auto gen ảnh có background màu và có chữ cái đầu của tên
                newUser.AvatarUrl = Utils.GenerateAvatar(newUser.Name);
                //Cập nhật pass
                if (!string.IsNullOrEmpty(newUser.Password))
                {
                    newUser.PasswordSalt = AccountHelper.CreatePasswordSalt();
                    newUser.Password = AccountHelper.HashPassword(newUser.Password, newUser.PasswordSalt);
                    newUser.PlainTextPwd = model.Password;
                }

                var newCauHinhNhanSu = new mk_CauHinhNhanSu
                {
                    IdUser = newUser.Id,
                };

                _dbContext.IdmUser.Add(newUser);
                _dbContext.mk_CauHinhNhanSu.Add(newCauHinhNhanSu);


                await _dbContext.SaveChangesAsync();

                // Create user wallet if neccesary


                return new Response(HttpStatusCode.OK, "Tạo tài khoản thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}, ApplicationIds: {@applicationId}", model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }
        public async Task<Response> RegisterAsync(UserRegisterRequestModel model)
        {
            try
            {
                var newUser = _mapper.Map<UserRegisterRequestModel, idm_User>(model);

                //Check trùng
                if (string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(model.FacebookUserId) &&
                    string.IsNullOrEmpty(model.FacebookUserId))
                    return new ResponseError(HttpStatusCode.BadRequest,
                        "Một trong các thông tin Email, FacebookId và GoogleId phải tồn tại");

                if (!string.IsNullOrEmpty(model.Email))
                {
                    var emailCheck = await _dbContext.IdmUser.Where(x => x.Email == model.Email).AnyAsync();
                    if (emailCheck)
                        return new ResponseError(HttpStatusCode.BadRequest,
                            "Email " + model.Email + " đã được sử dụng. Vui lòng lựa chọn một email khác.");
                }

                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    var phonelCheck =
                        await _dbContext.IdmUser.Where(x => x.PhoneNumber == model.PhoneNumber).AnyAsync();
                    if (phonelCheck)
                        return new ResponseError(HttpStatusCode.BadRequest,
                            "Số điện thoại " + model.PhoneNumber + " đã được sử dụng. Vui lòng lựa chọn số khác.");
                }

                if (model.Password != model.ConfirmPassword)
                {
                    return Helper.CreateBadRequestResponse("Xác nhận mật khẩu không khớp");
                }

                if (!_dbContext.IdmRole.Any(x => x.Code == model.Role))
                    return Helper.CreateBadRequestResponse("Không có loại người dùng này");
                // Set pre-defined user id
                if (!model.UserId.HasValue)
                    newUser.Id = Guid.NewGuid();
                else
                    newUser.Id = model.UserId.Value;

                newUser.Level = 1;
                newUser.Email = model.Email?.Trim().ToLower();
                newUser.Name = model.Name;
                newUser.FacebookUserId = model.FacebookUserId;
                newUser.AvatarUrl = model.AvatarUrl;
                newUser.GoogleUserId = model.GoogleUserId;
                newUser.Gender = model.Gender;
                newUser.RoleListCode = new List<string>();
                newUser.RoleListCode.Add(model.Role);
                // Set user name
                if (!string.IsNullOrEmpty(newUser.Email))
                    newUser.UserName = newUser.Email;
                else if (!string.IsNullOrEmpty(model.FacebookUserId))
                    newUser.UserName = model.FacebookUserId.ToLower();
                else if (!string.IsNullOrEmpty(model.GoogleUserId))
                    newUser.UserName = model.GoogleUserId.ToLower();
                newUser.PlainTextPwd = model.Password;
                newUser.StatusCode = UserStatusConstants.BUSY;
                newUser.MaPhongBan = model.MaPhongBan;
                newUser.MaTo = model.MaTo;
                
                if (model.Role == RoleConstants.AdminRoleCode)
                {
                    newUser.StatusCode = UserStatusConstants.FREE;
                }

                // mã hóa pass
                if (!string.IsNullOrEmpty(newUser.Password))
                {
                    newUser.PasswordSalt = AccountHelper.CreatePasswordSalt();
                    newUser.PlainTextPwd = newUser.Password;
                    newUser.Password = AccountHelper.HashPassword(newUser.Password, newUser.PasswordSalt);
                }

                newUser.IsEmailVerified = true;
                newUser.IsActive = true;
                newUser.IsLockedOut = false;
                newUser.ActiveDate = DateTime.Now;

                _dbContext.IdmUser.Add(newUser);

                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    // Get back
                    return new Response(HttpStatusCode.OK,
                        "Đăng kí tài khoản thành công");
                }
                else
                    return new Response(HttpStatusCode.InternalServerError, "Đăng kí tài khoản không thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@params}", model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> UpdateAsync(Guid id, UserUpdateModel model)
        {
            try
            {
                if (model.Ma != null)
                {
                    if (_dbContext.IdmUser.Any(x => x.Ma == model.Ma && x.Id != id))
                        return Helper.CreateBadRequestResponse(string.Format("Mã nhân sự {0} đã tồn tại", model.Ma));    
                }
                

                var request = Helper.GetRequestInfo(_httpContextAccessor);
                var userRepo = _dbContext.IdmUser;
                var currentUser = _dbContext.IdmUser.Where(x => x.Id == id).FirstOrDefault();
                if (currentUser == null)
                {
                    new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy người dùng");
                }

                if (!request.ListRoles.Contains(RoleConstants.AdminRoleCode) && request.UserId != currentUser.Id)
                {
                    return new ResponseError(HttpStatusCode.Forbidden,
                        "Bạn không được cập nhật profile của người khác");
                }

                //Check trùng
                var userCheck = await _dbContext.IdmUser.Where(x => x.UserName == model.UserName && x.Id != id)
                    .AnyAsync();
                if (userCheck)
                    return new ResponseError(HttpStatusCode.BadRequest,
                        "Tên người dùng đã được sử dụng bởi user khác.");

                var emailCheck = await _dbContext.IdmUser.Where(x => x.Email.Equals(model.Email) && x.Id != id)
                    .AnyAsync();
                if (emailCheck)
                    return new ResponseError(HttpStatusCode.BadRequest, "Email đã được sử dụng bởi user khác.");

                // Validate existing phone number
                var checkPhoneNumber = await _dbContext.IdmUser
                    .Where(x => x.PhoneNumber == model.PhoneNumber && x.Id != currentUser.Id).AnyAsync();
                if (checkPhoneNumber)
                    return new ResponseError(HttpStatusCode.BadRequest, "Số điện thoại đã được sử dụng bởi user khác.");

                if (model.RoleListCode != null && model.RoleListCode.Count() > 0)
                {
                    var listRoleCode = RoleCollection.Instance.Collection.Select(x => x.Code);

                    var exceptRole = model.RoleListCode.Intersect(listRoleCode);
                    if (exceptRole.Count() != model.RoleListCode.Count())
                        return Helper.CreateBadRequestResponse("Có nhóm người dùng không tồn tại trong hệ thống");
                }

                currentUser.Ma = !string.IsNullOrEmpty(model.Ma) ? model.Ma : currentUser.Ma;
                currentUser.Name = !string.IsNullOrEmpty(model.Name) ? model.Name : currentUser.Name;
                currentUser.PhoneNumber = !string.IsNullOrEmpty(model.PhoneNumber)
                    ? model.PhoneNumber
                    : currentUser.PhoneNumber;
                currentUser.UserName = !string.IsNullOrEmpty(model.UserName) ? model.UserName : currentUser.UserName;
                currentUser.Email = !string.IsNullOrEmpty(model.Email) ? model.Email : currentUser.Email;
                currentUser.IsLockedOut =
                    model.IsLockedOut.HasValue ? model.IsLockedOut.Value : currentUser.IsLockedOut;
                currentUser.IsActive = model.IsActive.HasValue ? model.IsActive.Value : currentUser.IsActive;
                currentUser.Gender = !string.IsNullOrEmpty(model.Gender) ? model.Gender.ToUpper() : currentUser.Gender;
                currentUser.RoleListCode = model.RoleListCode != null && model.RoleListCode.Count() > 0
                    ? model.RoleListCode
                    : currentUser.RoleListCode;
                if (model.IdChucVu.HasValue)
                {
                    currentUser.IdChucVu = model.IdChucVu;
                }
                // if (model.IdPhongBan.HasValue)
                // {
                //     currentUser.IdPhongBan = model.IdPhongBan;
                // }
                currentUser.MaPhongBan = model.MaPhongBan;
                currentUser.MaTo = model.MaTo;
                currentUser.Birthdate = model.Birthdate;
                currentUser.UserName = currentUser.Email;
                currentUser.LastModifiedByUserName = request.UserName;
                currentUser.LastModifiedByUserId = request.UserId;
                currentUser.LastModifiedOnDate = DateTime.Now;
                // set password

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    currentUser.PasswordSalt = AccountHelper.CreatePasswordSalt();
                    currentUser.Password = AccountHelper.HashPassword(model.Password, currentUser.PasswordSalt);
                    currentUser.PlainTextPwd = model.Password;
                }

                if (model.IsRevokedToken.HasValue && model.IsRevokedToken == true)
                {
                    _memoryCache.Remove(currentUser.Id);
                }

                await _dbContext.SaveChangesAsync();

                return new Response(HttpStatusCode.OK, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, Models: {@model}, ApplicationsIds: {@applicationId}", id, model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> UpdateInfoAsync(Guid userId, UserInfoUpdateModel model)
        {
            try
            {
                // Get current session language
                var user = Helper.GetRequestInfo(_httpContextAccessor);

                // Validate existing phone number
                var checkPhoneNumber = await _dbContext.IdmUser
                    .Where(x => x.PhoneNumber == model.PhoneNumber && x.Id != user.UserId).AnyAsync();
                if (checkPhoneNumber)
                    return new ResponseError(HttpStatusCode.BadRequest, "Số điện thoại đã được sử dụng.");

                var currentUser = _dbContext.IdmUser.Where(u => u.Id == userId).FirstOrDefault();

                // Do not update email
                // currentUser.Email = model.Email?.ToLower();
                currentUser.PhoneNumber = model.PhoneNumber;
                currentUser.Name = model.Name;
                currentUser.CountryCode = model.CountryCode;
                currentUser.BankAccountNo = model.BankAccountNo;
                currentUser.BankName = model.BankName;
                currentUser.BankUsername = model.BankUsername;
                currentUser.Gender = !string.IsNullOrWhiteSpace(model.Gender) ? model.Gender.ToUpper() : model.Gender;
                _dbContext.IdmUser.Update(currentUser);


                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    var result = await GetDetail(userId, user.Language);
                    result.Message = Utils.GetTranslation("response_message.update_info_success", user.Language);

                    return result;
                }
                else
                    return new Response(HttpStatusCode.OK, "Chỉnh sửa tài khoản thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}, Models: {@model}", userId, model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> DeleteAsync(Guid id)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var entity = await _dbContext.IdmUser.Where(x => x.Id == id).FirstOrDefaultAsync();
                // TODO
                //await _dbContext.SaveChangesAsync();
                if (entity == null)
                    return Helper.CreateNotFoundResponse<UserModel>("Người dùng không tồn tại");

                if (!currentUser.IsAdmin)
                    return Helper.CreateForbiddenResponse<UserModel>("Bạn không có quyền truy cập");


                _dbContext.Remove(entity);

                var status = await _dbContext.SaveChangesAsync();
                if (status > 0)
                    return new Response("Xóa thành công");
                return new Response("Xóa không thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> ForceChangePassword(Guid id, string password)
        {
            try
            {
                var currentUser = _dbContext.IdmUser.Where(s => s.Id == id).FirstOrDefault();
                if (currentUser == null)
                    return Helper.CreateNotFoundResponse("Không tìm thấy người dùng");

                currentUser.PasswordSalt = AccountHelper.CreatePasswordSalt();
                currentUser.Password = AccountHelper.HashPassword(password, currentUser.PasswordSalt);
                currentUser.PlainTextPwd = password;
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse("Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, Passwords:{@passwords}", id, password);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> ResetPassword(string resetPasswordToken, string password)
        {
            try
            {
                var currentUser = await _dbContext.IdmUser.Where(s => s.ResetPasswordToken == resetPasswordToken)
                    .FirstOrDefaultAsync();
                if (currentUser == null)
                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy người dùng");

                currentUser.PasswordSalt = AccountHelper.CreatePasswordSalt();
                currentUser.Password = AccountHelper.HashPassword(password, currentUser.PasswordSalt);
                currentUser.PlainTextPwd = password;
                currentUser.ResetPasswordToken = string.Empty;

                currentUser.UpdateLog = UpdateLogHelper.AddUpdateLog(string.Empty,
                    "Thay đổi mật khẩu",
                    string.Empty);

                await _dbContext.SaveChangesAsync();

                return new Response("Thay đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> ChangePasswordAsync(UserUpdatePasswordModel model)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var user = _dbContext.IdmUser.Where(x => x.Id == currentUser.UserId).FirstOrDefault();
                if (user == null)
                    return Helper.CreateNotFoundResponse("Không tìm thấy người dùng");

                // check old password
                var currentPassword = AccountHelper.HashPassword(model.OldPassword, user.PasswordSalt);
                if (currentPassword != user.Password)
                {
                    return new ResponseError(HttpStatusCode.BadRequest, "Mật khẩu cũ không chính xác");
                }

                if (model.ConfirmPassword != model.Password)
                {
                    return Helper.CreateBadRequestResponse("Xác nhận mật khẩu không khớp");
                }

                user.PasswordSalt = AccountHelper.CreatePasswordSalt();
                user.Password = AccountHelper.HashPassword(model.Password, user.PasswordSalt);
                user.PlainTextPwd = model.Password;

                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse("Đổi mật khẩu thành công!");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id:, OldPasswords:{@oldPassword}, Passwords: {@password}", model.OldPassword,
                    model.Password);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        #endregion CRUD


        public async Task<Response> ForgotPassword(string email)
        {
            try
            {
                email = email.Trim().Replace(" ", string.Empty);
                var currentUser = _dbContext.IdmUser.Where(s => s.Email == email).FirstOrDefault();
                if (currentUser == null)
                    return new Response(HttpStatusCode.NotFound, "Email " + email + " chưa được đăng kí với hệ thống");

                // Create password reset token
                var resetPasswordToken = Utils.RandomInt(6);
                while (_dbContext.IdmUser.Any(x => x.ResetPasswordToken == resetPasswordToken))
                {
                    resetPasswordToken = Utils.RandomInt(6);
                }

                currentUser.ResetPasswordToken = resetPasswordToken;
                await _dbContext.SaveChangesAsync();

                var mailSendStatus = await SendForgotPasswordEmail(currentUser);

                return new Response(HttpStatusCode.OK,
                    "Hệ thống đã gửi thông tin, vui lòng kiểm tra email hoặc tin nhắn.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: PhoneNumber: {@params}", email);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> VerifyForgotPasswordToken(string resetPasswordToken)
        {
            try
            {
                var currentUser = await _dbContext.IdmUser.Where(s => s.ResetPasswordToken == resetPasswordToken)
                    .FirstOrDefaultAsync();
                // check password
                if (currentUser == null)
                    return Helper.CreateBadRequestResponse("Mã xác thực không đúng");

                return new Response(HttpStatusCode.OK, "Xác thực thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Token: {@params}", resetPasswordToken);
                return Utils.CreateExceptionResponseError(ex);
            }
        }


        public async Task<Response> ChangeLockStatusAsync(Guid id, bool status, Guid byUserId)
        {
            try
            {
                var currentUser = _dbContext.IdmUser.Where(x => x.Id == id).FirstOrDefault();
                if (currentUser == null)
                    return Helper.CreateNotFoundResponse("Không tìm thấy người dùng");

                currentUser.IsLockedOut = status;
                currentUser.IsActive = !status;
                await _dbContext.SaveChangesAsync();
                _memoryCache.Remove(currentUser.Id);
                if (status)
                    return Helper.CreateSuccessResponse("Đóng tài khoản thành công");
                else
                    return Helper.CreateSuccessResponse("Mở tài khoản thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, Status: {@status}, ByUserIds: {@requestUserId}", id, status,
                    byUserId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> ChangeActiveStatusAsync(Guid id, bool status, Guid byUserId)
        {
            try
            {
                var currentUser = _dbContext.IdmUser.Where(x => x.Id == id).FirstOrDefault();
                if (currentUser == null)
                    return Helper.CreateNotFoundResponse("Không tìm thấy người dùng");

                currentUser.IsActive = status;

                await _dbContext.SaveChangesAsync();

                if (status)
                    return Helper.CreateSuccessResponse("User has been active!");
                else
                    return Helper.CreateSuccessResponse("User has been deactive.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, Status: {@status}, ByUserIds: {@requestUserId}", id, status,
                    byUserId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }


        public async Task<Response> UpdateAvatarUrl(UserSetAvatarUrlModel model)
        {
            try
            {
                //check null
                if (string.IsNullOrEmpty(model.AvatarUrl))
                    return new ResponseError(HttpStatusCode.BadRequest, "Không được để trống Avatar Url");
                //check file existence
                var hostFolder = Utils.GetConfig("StaticFiles:Folder");
                /*if (!File.Exists(Path.Combine(hostFolder, model.AvatarUrl)))
                    return new Response(HttpStatusCode.BadRequest, "File chưa tồn tại trên máy chủ.");*/
                //check existed user
                var user = await _dbContext.IdmUser.FirstOrDefaultAsync(u => u.Id == model.UserId);
                if (user == null)
                    return new Response(HttpStatusCode.NotFound, "Không tìm thấy user");
                //input
                user.AvatarUrl = model.AvatarUrl;

                // Save changes
                await _dbContext.SaveChangesAsync();

                var result = await GetDetail(model.UserId, model.AvatarUrl);
                result.Message = "Cập nhật ảnh đại diện thành công";
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@params}", model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response<LoginResponse>> SwitchProfile(ProfileTypeModel model)
        {
            try
            {
                //check null
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var user = await _dbContext.IdmUser.FirstOrDefaultAsync(x => x.Id == currentUser.UserId);
                if (user == null)
                    return Helper.CreateNotFoundResponse<LoginResponse>("Không có người dùng này");
                if (!_profileTypes.Contains(model.ProfileType))
                    return Helper.CreateBadRequestResponse<LoginResponse>("Không có loại profile này");


                var listRoleCode = new List<string>();
                if (model.ProfileType == SendProposalTypeConstants.FARMER)
                {
                    user.StatusCode = UserStatusConstants.FREE;
                    listRoleCode.Add(RoleConstants.FarmerSideRoleCode);
                }
                else
                {
                    listRoleCode.Add(RoleConstants.OrderSideRoleCode);
                    user.StatusCode = UserStatusConstants.BUSY;
                }

                user.RoleListCode = listRoleCode;
                await _dbContext.SaveChangesAsync();

                return await buildLoginResponse(user, true, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@params}", model);
                return Helper.CreateExceptionResponse<LoginResponse>(ex);
            }
        }

        public async Task<Response> RevokeToken(Guid id)
        {
            try
            {
                //check null
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var user = _memoryCache.Get<UserAuthInfo>(id);
                if (user != null)
                    _memoryCache.Remove(id);

                return Helper.CreateSuccessResponse("Thu hồi token thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<LoginResponse>(ex);
            }
        }

        public async Task<Response> UpdateLocation(UserLocationrUpdateModel model)
        {
            try
            {
                //check null
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var entity = await _dbContext.IdmUser.Where(x => x.Id == currentUser.UserId).FirstOrDefaultAsync();
                if (entity == null)
                    return Helper.CreateNotFoundResponse("Không có người dùng này");
                // entity.Location = new Point(new Coordinate(model.Lat, model.Long)) { SRID = 4326 };
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse("Cập nhật ví trị thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<LoginResponse>(ex);
            }
        }

        public async Task<Response> SwitchStatus(UserStatusUpdateModel model)
        {
            try
            {
                //check null
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var entity = await _dbContext.IdmUser.Where(x => x.Id == currentUser.UserId).FirstOrDefaultAsync();
                if (entity == null)
                    return Helper.CreateNotFoundResponse("Không có người dùng này");
                entity.StatusCode = model.StatusCode;
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse("Cập nhật trạng thái này công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<LoginResponse>(ex);
            }
        }

        public async Task<Response> Logout(string isMobileDevice)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var user = _dbContext.IdmUser.Where(x => x.Id == currentUser.UserId).FirstOrDefault();
                if (user == null)
                    return new ResponseError(HttpStatusCode.NotFound, "User not found");
                var userinfo = _memoryCache.Get<UserAuthInfo>(currentUser.UserId);
                if (userinfo != null)
                {
                    _memoryCache.Remove(currentUser.UserId);
                }

                if (!string.IsNullOrEmpty(isMobileDevice))
                {
                    user.DeviceToken = isMobileDevice == "true" ? null : user.DeviceToken;
                    await _dbContext.SaveChangesAsync();
                }

                return new Response();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);

                return Common.Utils.CreateExceptionResponseError(ex);
            }
        }

        private async Task<Response> SendForgotPasswordEmail(idm_User user)
        {
            try
            {
                if (user == null)
                    new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy người dùng");

                // Generate reset token and link

                // Load email content
                // var link = Utils.GetConfig("HostUrl") + "auth/resetpassword?token=" + user.ResetPasswordToken;

                var emailContent =
                    string.Format(System.IO.File.ReadAllText(@"Resources/EmailTemplate/Reset_Password_Email.html"),
                        user.ResetPasswordToken);

                var message = new EmailMessage
                {
                    To = new[] { user.Email },
                    Subject = "Reset your password on GENEAT",
                    Content = emailContent
                };

                var sendMailResult = await _emailService.SendAsync(message);
                if (sendMailResult)
                    return new Response(HttpStatusCode.OK, "Gửi mail thành công");
                else
                    return new Response(HttpStatusCode.OK, "Gửi mail không thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: user: {@params}", user);
                return Utils.CreateExceptionResponseError(ex);
            }
        }
    }
}