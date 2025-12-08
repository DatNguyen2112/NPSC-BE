using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace NSPC.Common
{
    public class Helper
    {
        /// <summary>
        /// Transform data to http response
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ActionResult TransformData(Response data)
        {
            var result = new ObjectResult(data) { StatusCode = (int)data.Code };
            return result;
        }

        /// <summary>
        /// Get user info in token and header
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static RequestUser GetRequestInfo(HttpRequest request)
        {
            try
            {
                var result = new RequestUser
                {
                    IsAuthenticated = false,
                    UserId = Guid.Empty,
                    UserName = "",
                    ApplicationId = Guid.Empty
                };

                // Language
                request.Headers.TryGetValue("x-language", out StringValues language);
                if (string.IsNullOrEmpty(language))
                {
                    language = NSPCConstants.LanguageConstants.Default;
                    result.Language = language;
                }
                else
                {
                    string lang = language;
                    lang = lang.ToLower();
                    result.Language = NSPCConstants.LanguageConstants.Default;

                    if (lang != NSPCConstants.LanguageConstants.Vietnamese && lang != NSPCConstants.LanguageConstants.English && lang != NSPCConstants.LanguageConstants.Japanese && lang != NSPCConstants.LanguageConstants.Chinese)
                        lang = NSPCConstants.LanguageConstants.Vietnamese;

                }

                // Currency
                request.Headers.TryGetValue("x-currency", out StringValues currency);
                if (string.IsNullOrEmpty(currency))
                {
                    currency = NSPCConstants.CurrencyConstants.Default;
                    result.Currency = currency;
                }
                else
                {
                    result.Currency = ((string)currency).ToUpper();
                }

                request.Headers.TryGetValue("X-Permission", out StringValues currentToken);
                if (string.IsNullOrEmpty(currentToken))
                {
                    var token = request.Headers["Authorization"].ToString();
                    if (!string.IsNullOrEmpty(token))
                    {
                        var tokenString = token.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                        if (tokenString.Length > 1)
                        {
                            currentToken = tokenString[1]?.Trim();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(currentToken))
                {
                    string secret = Utils.GetConfig("Authentication:Jwt:Key");
                    var key = Encoding.ASCII.GetBytes(secret);
                    var handler = new JwtSecurityTokenHandler();
                    var validations = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = Utils.GetConfig("Authentication:Jwt:Issuer"),
                        ValidAudience = Utils.GetConfig("Authentication:Jwt:Issuer"),
                    };
                    var currentUser = handler.ValidateToken(currentToken, validations, out var tokenSecure);
                    //result.ExpireAt = tokenSecure.ValidTo;

                    //UserId
                    if (currentUser.HasClaim(c => c.Type == NSPCConstants.ClaimConstants.USER_ID))
                    {
                        var userId = currentUser.Claims.FirstOrDefault(c => c.Type == NSPCConstants.ClaimConstants.USER_ID)?.Value;
                        if (!string.IsNullOrEmpty(userId) && Utils.IsGuid(userId))
                        {
                            result.UserId = new Guid(userId);
                        }
                    }
                    else
                    {
                        request.Headers.TryGetValue("X-UserId", out StringValues userId);
                        if (!string.IsNullOrEmpty(userId) && Utils.IsGuid(userId))
                        {
                            result.UserId = new Guid(userId);
                        }
                    }

                    //UserName
                    if (currentUser.HasClaim(c => c.Type == NSPCConstants.ClaimConstants.USER_NAME))
                    {
                        var userName = currentUser.Claims.FirstOrDefault(c => c.Type == NSPCConstants.ClaimConstants.USER_NAME)?.Value;
                        if (!string.IsNullOrEmpty(userName))
                        {
                            result.UserName = userName;
                        }
                    }

                    //LoginName
                    if (currentUser.HasClaim(c => c.Type == NSPCConstants.ClaimConstants.FULL_NAME))
                    {
                        var fullName = currentUser.Claims.FirstOrDefault(c => c.Type == NSPCConstants.ClaimConstants.FULL_NAME)?.Value;
                        if (!string.IsNullOrEmpty(fullName))
                        {
                            result.FullName = fullName;
                        }
                    }

                    //PhoneNumber
                    if (currentUser.HasClaim(c => c.Type == NSPCConstants.ClaimConstants.PHONE))
                    {
                        var phoneNumber = currentUser.Claims.FirstOrDefault(c => c.Type == NSPCConstants.ClaimConstants.PHONE)?.Value;
                        if (!string.IsNullOrEmpty(phoneNumber))
                        {
                            result.PhoneNumber = phoneNumber;
                        }
                    }
                    else
                    {
                        request.Headers.TryGetValue("X-phone", out StringValues phoneNumber);
                        if (!string.IsNullOrEmpty(phoneNumber))
                        {
                            result.PhoneNumber = phoneNumber;
                        }
                    }

                    if (currentUser.HasClaim(c => c.Type == ClaimConstants.TENANT_ID))
                    {
                        var tenantIdString = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimConstants.TENANT_ID)?.Value;
                        if (!string.IsNullOrEmpty(tenantIdString))
                        {
                            Guid.TryParse(tenantIdString, out var tenantId);
                            result.TenantId = tenantId;
                        }
                    }

                    // //AppId
                    // if (currentUser.HasClaim(c => c.Type == ClaimConstants.APP_ID))
                    // {
                    //     var appId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimConstants.APP_ID)?.Value;
                    //     if (!string.IsNullOrEmpty(appId) && Utils.IsGuid(appId))
                    //     {
                    //         result.ApplicationId = new Guid(appId);
                    //     }
                    // }
                    // else
                    // {
                    request.Headers.TryGetValue("X-ApplicationId", out StringValues applicationId);
                    if (!string.IsNullOrEmpty(applicationId) && Utils.IsGuid(applicationId))
                    {
                        result.ApplicationId = new Guid(applicationId);
                    }
                    else
                    {
                        if (currentUser.HasClaim(c => c.Type == NSPCConstants.ClaimConstants.APP_ID))
                        {
                            var appId = currentUser.Claims.FirstOrDefault(c => c.Type == NSPCConstants.ClaimConstants.APP_ID)?.Value;
                            if (!string.IsNullOrEmpty(appId) && Utils.IsGuid(appId))
                            {
                                result.ApplicationId = new Guid(appId);
                            }
                        }
                    }
                    // }

                    //ListRoles
                    var listRoles = currentUser.Claims.FirstOrDefault(c => c.Type == NSPCConstants.ClaimConstants.ROLES)?.Value;
                    if (!string.IsNullOrEmpty(listRoles))
                    {
                        result.ListRoles = JsonConvert.DeserializeObject<List<string>>(listRoles);
                    }
                    //ListRights
                    var listRights = currentUser.Claims.FirstOrDefault(c => c.Type == NSPCConstants.ClaimConstants.RIGHTS)?.Value;
                    if (!string.IsNullOrEmpty(listRights))
                    {
                        result.ListRights = JsonConvert.DeserializeObject<List<string>>(listRights);
                    }

                    // Level
                    var levelClaim = currentUser.Claims.FirstOrDefault(c => c.Type == NSPCConstants.ClaimConstants.LEVEL);
                    result.Level = int.Parse(levelClaim != null ? levelClaim.Value : "1");

                    result.IsNormalUser = result.ListRoles.Contains(RoleConstants.NormalUser);
                    result.IsAdmin = result.ListRoles.Contains(RoleConstants.AdminRoleCode);
                    result.IsNND = result.ListRoles.Contains(RoleConstants.NND);
                    result.IsStaff = result.ListRoles.Contains(RoleConstants.StaffCode);

                    if (currentUser.HasClaim(c => c.Type == NSPCConstants.ClaimConstants.PARTNER_ID))
                    {
                        var partnerId = currentUser.Claims.FirstOrDefault(c => c.Type == NSPCConstants.ClaimConstants.PARTNER_ID)?.Value;
                        if (!string.IsNullOrEmpty(partnerId) && Utils.IsGuid(partnerId))
                        {
                            result.PartnerId = new Guid(partnerId);
                        }
                    }
                    else
                    {
                        request.Headers.TryGetValue("X-Partner-Id", out StringValues partnerId);
                        if (!string.IsNullOrEmpty(partnerId) && Utils.IsGuid(partnerId))
                        {
                            result.PartnerId = new Guid(partnerId);
                        }
                    }

                    /* result.IsNPApprover = result.ListRoles.Contains(RoleConstants.NPApproverCode);
                     result.IsNPComposer = result.ListRoles.Contains(RoleConstants.NPComposerCode);
                     result.IsNPPublisher = result.ListRoles.Contains(RoleConstants.NPPublisherCode);
                     result.IsNPAdmin = result.ListRoles.Contains(RoleConstants.NPAdminCode);
                     if (result.ListRoles.Contains(RoleConstants.PostFullControlRoleCode) || result.ListRoles.Contains(RoleConstants.AdminRoleCode))
                         result.IsNPAdmin = true;*/
                }
                else
                {
                    request.Headers.TryGetValue("X-ApplicationId", out StringValues applicationId);
                    if (!string.IsNullOrEmpty(applicationId) && Utils.IsGuid(applicationId))
                    {
                        result.ApplicationId = new Guid(applicationId);
                    }
                    request.Headers.TryGetValue("X-UserId", out StringValues userId);
                    if (!string.IsNullOrEmpty(userId) && Utils.IsGuid(userId))
                    {
                        result.UserId = new Guid(userId);
                    }
                    request.Headers.TryGetValue("X-UserName", out StringValues userName);
                    if (!string.IsNullOrEmpty(userName))
                    {
                        result.UserName = userName;
                    }
                    request.Headers.TryGetValue("Origin", out StringValues domain);
                    if (!string.IsNullOrEmpty(domain))
                    {
                        var Uri = new Uri(domain);
                        result.Domain = Uri.Host;
                        result.SubDomain = Utils.GetSubDomain(Uri.Host, Utils.GetConfig("Authentication:Domain"));
                    }
                }

                // result.IsNormalUser = false;
                // result.IsNormalUser = result.Level == 1;
                // result.IsAdmin = false;
                // result.IsPartner = false;
                // result.IsAdmin = result.Level == 10;
                // result.IsPartner = result.Level == 5;
                if (result.UserId != Guid.Empty)
                    result.IsAuthenticated = true;

                return result;
            }
            catch (Exception exx)
            {
                Log.Error(exx, string.Empty);
                throw;
            }
        }


        /// <summary>
        /// Get user info using a HttpContext
        /// </summary>
        /// <param name="_httpContextAccessor"></param>
        /// <returns></returns>
        public static RequestUser GetRequestInfo(IHttpContextAccessor _httpContextAccessor)
        {
            try
            {
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
                {
                    var requestInfo = GetRequestInfo(_httpContextAccessor.HttpContext.Request);
                    return requestInfo;
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return null;
            }
        }

        public static Response<T> CreateSuccessResponse<T>(T data)
        {
            return new Response<T>(HttpStatusCode.OK, data, "Success");
        }

        public static Response<T> CreateSuccessResponse<T>(T data, string message)
        {
            return new Response<T>(HttpStatusCode.OK, data, message);
        }
        public static Response CreateSuccessResponse()
        {
            return new Response(HttpStatusCode.OK, "Success");
        }

        public static Response CreateSuccessResponse(string message)
        {
            return new Response(HttpStatusCode.OK, message);
        }

        /// <summary>
        /// Create a generic not found response to resource with class type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Response<T> CreateNotFoundResponse<T>()
        {
            //return new Response<T>(HttpStatusCode.NotFound, default(T), "The requested resource doesn't exist.");
            return new Response<T>(HttpStatusCode.NotFound, default(T), "");
        }

        public static Response<T> CreateNotFoundResponse<T>(string message)
        {
            //  return new Response<T>(HttpStatusCode.NotFound, default(T), "The requested resource doesn't exist. " + message);
            return new Response<T>(HttpStatusCode.NotFound, default(T), "" + message);
        }

        public static Response CreateNotFoundResponse()
        {
            // return new Response(HttpStatusCode.NotFound, "The requested resource doesn't exist.");
            return new Response(HttpStatusCode.NotFound, "The requested resource doesn't exist.");
        }

        public static Response CreateNotFoundResponse(string message)
        {
            return new Response(HttpStatusCode.NotFound, "The requested resource doesn't exist. " + message);
        }

        /// <summary>
        /// Create a generic not found response to resource with class type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Response<T> CreateExceptionResponse<T>(Exception ex)
        {
            return new Response<T>(HttpStatusCode.InternalServerError, default(T), Utils.CreateExceptionMessage(ex));
        }

        public static Response<T> CreateExceptionResponse<T>(string message, Exception ex)
        {
            return new Response<T>(HttpStatusCode.InternalServerError, default(T), "Error: " + message + " / " + Utils.CreateExceptionMessage(ex));
        }
        public static Response CreateExceptionResponse(Exception ex)
        {
            return new Response(HttpStatusCode.InternalServerError, Utils.CreateExceptionMessage(ex));
        }
        public static Response CreateExceptionResponse(string message)
        {
            return new Response(HttpStatusCode.InternalServerError, message);
        }


        /// <summary>
        /// Create a generic forbidden response to resource with class type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Response<T> CreateForbiddenResponse<T>()
        {
            return new Response<T>(HttpStatusCode.Forbidden, default(T), "You don't have access to this resource.");
        }

        /// <summary>
        /// Create a generic forbidden response to resource with class type and message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Response<T> CreateForbiddenResponse<T>(string message)
        {
            //    return new Response<T>(HttpStatusCode.Forbidden, default(T), "You don't have access to this resource. " + message);
            return new Response<T>(HttpStatusCode.Forbidden, default(T), "" + message);
        }

        /// <summary>
        /// Create a generic forbidden response to resource
        /// </summary>
        /// <returns></returns>
        public static Response CreateForbiddenResponse()
        {
            return new Response(HttpStatusCode.Forbidden, "You don't have access to this resource.");
        }

        /// <summary>
        /// Create a generic forbidden response to resource
        /// </summary>
        /// <returns></returns>
        public static Response CreateForbiddenResponse(string message)
        {
            return new Response(HttpStatusCode.Forbidden, "You don't have access to this resource. " + message);
        }

        /// <summary>
        /// Create a generic Bad Request response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Response<T> CreateBadRequestResponse<T>()
        {
            return new Response<T>(HttpStatusCode.BadRequest, default(T), "Your requested data is invalid or malformed.");
        }

        /// <summary>
        /// Create a generic Bad Request response message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Response<T> CreateBadRequestResponse<T>(string message)
        {
            //  return new Response<T>(HttpStatusCode.BadRequest, default(T), "Your requested data is invalid or malformed. Reason/message: " + message);
            return new Response<T>(HttpStatusCode.BadRequest, default(T), "" + message);
        }

        /// <summary>
        /// Create a generic bad request response to resource
        /// </summary>
        /// <returns></returns>
        public static Response CreateBadRequestResponse()
        {
            return new Response(HttpStatusCode.BadRequest, "Your requested data is invalid or malformed.");
        }

        public static Response CreateBadRequestResponse(string message)
        {
            //  return new Response(HttpStatusCode.BadRequest, "Your requested data is invalid or malformed.  Reason/message: " + message);
            return new Response(HttpStatusCode.BadRequest, "" + message);
        }


        public static Response<T> CreateUnauthorizedResponse<T>()
        {
            return new Response<T>(HttpStatusCode.Unauthorized, default(T), "Your requested is unauthorized.");
        }
        public static Response<T> CreateUnauthorizedResponse<T>(string message)
        {
            return new Response<T>(HttpStatusCode.Unauthorized, default(T), message);
        }
        public static Response CreateUnauthorizedResponse(string message)
        {
            return new Response(HttpStatusCode.Unauthorized, message);
        }


        public class RequestUser
        {

            public Guid UserId { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }

            public string MaPhongBan { get; set; }
            public string MaTo { get; set; }

            /// <summary>
            /// Application ID
            /// </summary>
            public Guid ApplicationId { get; set; }

            public List<string> ListRoles { get; set; }
            public List<string> ListRights { get; set; }
            public bool IsAdmin { get; set; }
            public bool IsPartner { get; set; }
            public bool IsNND { get; set; }
            public bool IsStaff { get; set; }
            /// <summary>
            /// Normal user, level = 1
            /// </summary>
            public bool IsNormalUser { get; set; }
            public int Level { get; set; }
            public string Language { get; set; }
            public string Currency { get; set; }
            public Guid? PartnerId { get; set; }
            public DateTime ExpireAt { get; set; }

            public bool IsAuthenticated { get; set; }
            public Guid? TenantId { get; set; }
            public string Domain { get; set; }
            public string SubDomain { get; set; }
            public RequestUser()
            {

            }

            /// <summary>
            /// Quickly create a request user
            /// </summary>
            /// <param name="userId"></param>
            /// <param name="userName"></param>
            public RequestUser(Guid userId, string userName)
            {
                UserId = userId;
                UserName = userName;
            }

            /// <summary>
            /// Quickly create a request user
            /// </summary>
            /// <param name="userId"></param>
            public RequestUser(string stringUserId)
            {
                UserId = Guid.Parse(stringUserId);
                UserName = string.Empty;
            }


        }
    }
}