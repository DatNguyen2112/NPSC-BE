using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using Google.Apis.Auth;
using NSPC.Business;
using NSPC.Common;
using NSPC.Data;
using NSPC.Api;
using NSPC.Data.Data;

namespace NSPC.Api
{
    /// <summary>
    /// JWT cho hệ thống
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/authentication")]
    [ApiExplorerSettings(GroupName = "Admin Authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserHandler _userHandler;
        private readonly FacebookService _facebookService;
        private readonly SMDbContext _dbContext;
        private static readonly HttpClient Client = new HttpClient();

        public AuthenticationController(IConfiguration config, IUserHandler userHandler, FacebookService facebookService, SMDbContext dbContext)
        {
            _config = config;
            _userHandler = userHandler;
            _facebookService = facebookService;
            _dbContext = dbContext;

        }

        /// <summary>
        /// Đăng kí tài khoản
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpPost, Route("register")]
        [ProducesResponseType(typeof(Response<UserModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterRequestModel model)
        {
            // Fill language
            var languageCode = Request.Headers[ClaimConstants.LANGUAGE].FirstOrDefault();
            model.Language = languageCode != null ? languageCode.ToLower() : LanguageConstants.English;

            // Fill currency
            var currency = Request.Headers[ClaimConstants.CURRENCY].FirstOrDefault();
            model.Currency = currency != null ? currency.ToUpper() : NSPCConstants.CurrencyConstants.Default;

            var result = await _userHandler.RegisterAsync(model);

            //// Success
            //if (result.Code == System.Net.HttpStatusCode.OK)
            //{
            //    var userId = (result as Response<UserModel>).Data.Id;

            //    // Login
            //    return await _userHandler.BuildToken(userId, true);
            //}

            // Hander response
            return Helper.TransformData(result);
        }

        #region JWT

        /// <summary>
        /// Lấy thông tin jwt
        /// </summary>
        /// <returns></returns>
        [Route("jwt/info")]
        [Authorize, HttpPost]
        [ProducesResponseType(typeof(Response<LoginResponse>), StatusCodes.Status200OK)]
        public async Task<Response<LoginResponse>> JwtInfo()
        {
            var requestInfo = Helper.GetRequestInfo(Request);

            return await _userHandler.BuildLoginResponse(requestInfo.UserId, true);
        }

        /// <summary>
        /// Đăng nhập và lấy kết quả JWT token
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [Route("jwt/login")]
        [AllowAnonymous, HttpPost]
        [ProducesResponseType(typeof(Response<LoginResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SignInJwt([FromBody] LoginModel login)
        {
            IActionResult response = Unauthorized();

            var userAuthResponse = await _userHandler.Authentication(login.Username, login.Password, login.RememberMe, login.DeviceToken);

            if (userAuthResponse.IsSuccess)
            {
                userAuthResponse.Message = "Đăng nhập thành công";
                return Helper.TransformData(userAuthResponse);
            }    
            else
                return Helper.TransformData(Helper.CreateForbiddenResponse<LoginResponse>(userAuthResponse.Message));
        }

        #endregion JWT

        #region Facebook

        /// <summary>
        /// Đăng nhập qua FB và lấy kết quả JWT token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("fb/login")]
        [AllowAnonymous, HttpPost]
        public async Task<Response<LoginResponse>> SignInFacebook([FromBody] FacebookLoginRequest model)
        {
            var facebookUser = await _facebookService.GetUserFromFacebookAsync(model.AccessToken);

            if (facebookUser == null)
                return Helper.CreateUnauthorizedResponse<LoginResponse>("Người dùng chưa được xác thực với Facebook");


            // Log
            Log.Information("Start facebook user login");
            Log.Information("Facebook user login data: {@0}", model);
            Log.Information("Facebook user login response: {@0}", facebookUser);
            Log.Information("Facebook user login email: {@0}", facebookUser?.Email);

            var user = await _dbContext.IdmUser.Where(x => (facebookUser.Email != null && x.Email == facebookUser.Email) || x.FacebookUserId == facebookUser.UserId).FirstOrDefaultAsync();

            // Download avatar
            var avatarUrl = string.Empty;
            Guid newUserId = Guid.NewGuid();

            if (user != null)
                avatarUrl = await Utils.DownloadAvatar(facebookUser.Picture, user.Id.ToString().Substring(0, 8));
            else
                avatarUrl = await Utils.DownloadAvatar(facebookUser.Picture, newUserId.ToString().Substring(0, 8));

            if (user == null)
            {
                // Register new user
                // Fill language
                var registerModel = new UserRegisterRequestModel
                {
                    Email = facebookUser.Email,
                    Password = AccountHelper.CreatePassword(10),
                    FacebookUserId = facebookUser.UserId,
                    AvatarUrl = avatarUrl,
                    Name = facebookUser.Name,
                    UserId = newUserId,
                };

                var languageCode = Request.Headers[ClaimConstants.LANGUAGE].FirstOrDefault();
                registerModel.Language = languageCode != null ? languageCode.ToLower() : LanguageConstants.English;

                // Fill currency
                var currency = Request.Headers[ClaimConstants.CURRENCY].FirstOrDefault();
                registerModel.Currency = currency != null ? currency.ToUpper() : NSPCConstants.CurrencyConstants.Default;

                var result = await _userHandler.RegisterAsync(registerModel);

                // Success
                if (result.Code == System.Net.HttpStatusCode.OK)
                {
                    var userId = (result as Response<UserModel>).Data.Id;
                    // Login
                    return await _userHandler.BuildLoginResponse(userId, true);
                }
                else
                    return Helper.CreateBadRequestResponse<LoginResponse>(result.Message);
            }
            else
            {
                // UpdatePaid username
                user.Name = facebookUser.Name;
                user.AvatarUrl = avatarUrl;
                user.FacebookUserId = facebookUser.UserId;
                user.Email = facebookUser.Email;
                user.IsEmailVerified = true;
                user.IsActive = true;
                if (!user.ActiveDate.HasValue)
                    user.ActiveDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();

                return await _userHandler.BuildLoginResponse(user.Id, false);
            }

        }
        #endregion Facebook

        /// <summary>
        /// Đăng nhập với Google
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("gg/login")]
        [AllowAnonymous, HttpPost]
        public async Task<Response<LoginResponse>> GoogleLoginAsync([FromBody] GoogleLoginRequest request)
        {

            try
            {
                var googleUser = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new ValidationSettings
                {
                    Audience = new[] { Utils.GetConfig("Authentication:Google:ClientId") },
                    ExpirationTimeClockTolerance = new TimeSpan(0, 0, 5)
                });

                if (googleUser == null)
                    return Helper.CreateUnauthorizedResponse<LoginResponse>("Người dùng chưa được xác thực với Google");

                Log.Information("Start google user login");
                Log.Information("Google user login data: {@0}", request);
                Log.Information("Google user login response: {@0}", googleUser);
                Log.Information("Google user login email: {@0}", googleUser?.Email);

                var user = await _dbContext.IdmUser.Where(x => (x.Email == googleUser.Email && googleUser.Email != null) || x.GoogleUserId == googleUser.Subject).FirstOrDefaultAsync();

                // Download avatar
                var avatarUrl = await Utils.DownloadAvatar(googleUser.Picture, Guid.NewGuid().ToString().Substring(0, 8));

                if (user == null)
                {
                    // Register new user
                    // Fill language
                    var registerModel = new UserRegisterRequestModel
                    {
                        Email = googleUser.Email,
                        Password = AccountHelper.CreatePassword(10),
                        GoogleUserId = googleUser.Subject,
                        AvatarUrl = avatarUrl,
                        Name = googleUser.Name,
                    };

                    var languageCode = Request.Headers[ClaimConstants.LANGUAGE].FirstOrDefault();
                    registerModel.Language = languageCode != null ? languageCode.ToLower() : LanguageConstants.English;

                    // Fill currency
                    var currency = Request.Headers[ClaimConstants.CURRENCY].FirstOrDefault();
                    registerModel.Currency = currency != null ? currency.ToUpper() : NSPCConstants.CurrencyConstants.Default;

                    var result = await _userHandler.RegisterAsync(registerModel);

                    // Success
                    if (result.Code == System.Net.HttpStatusCode.OK)
                    {
                        var userId = (result as Response<UserModel>).Data.Id;

                        // Login
                        return await _userHandler.BuildLoginResponse(userId, true);
                    }
                    else
                        return Helper.CreateBadRequestResponse<LoginResponse>(result.Message);
                }
                else
                {
                    // UpdatePaid username
                    user.Name = googleUser.Name;
                    user.AvatarUrl = avatarUrl;
                    user.GoogleUserId = googleUser.Subject;
                    user.IsEmailVerified = true;
                    if (!user.ActiveDate.HasValue)
                        user.ActiveDate = DateTime.Now;

                    await _dbContext.SaveChangesAsync();

                    return await _userHandler.BuildLoginResponse(user.Id, false);
                }

            }
            catch (Exception ex)
            {
                return Helper.CreateExceptionResponse<LoginResponse>(ex);
            }
        }

        /// <summary>
        /// Cấu hình theme
        /// </summary>
        /// <returns></returns>
        [Route("config-theme")]
        [HttpPost, Authorize]
        [ProducesResponseType(typeof(Response<LoginResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfigsTheme(ThemeConfigModel config, Guid userId)
        {
            // var requestInfo = Helper.GetRequestInfo(Request);

            var result = await _userHandler.UpdateThemeConfig(config,userId);


            return Helper.TransformData(result);
        }

        /// <summary>
        /// remove token from cache
        /// </summary>
        /// <returns></returns>
        [Route("logout")]
        [HttpPost, Authorize]
        [ProducesResponseType(typeof(Response<LoginResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout(string isMobileDevice)
        {
            // var requestInfo = Helper.GetRequestInfo(Request);

            var result = await _userHandler.Logout(isMobileDevice);


            return Helper.TransformData(result);
        }
    }
}