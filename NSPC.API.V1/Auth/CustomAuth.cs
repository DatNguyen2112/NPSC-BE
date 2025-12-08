using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using NSPC.Common;
using static NSPC.Common.Constants;

namespace NSPC.API.V1.Auth
{
    public static class CustomAuthExtensions
    {
        public static AuthenticationBuilder AddCustomAuth(this AuthenticationBuilder builder, Action<CustomAuthOptions> configureOptions)
        {
            return builder.AddScheme<CustomAuthOptions, CustomAuthHandler>("Custom Scheme", "Custom Auth", configureOptions);
        }
    }

    public class CustomAuthOptions : AuthenticationSchemeOptions
    {
        public CustomAuthOptions()
        {
        }
    }

    internal class CustomAuthHandler : AuthenticationHandler<CustomAuthOptions>
    {
        public CustomAuthHandler(IOptionsMonitor<CustomAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            // store custom services here...
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            string authHeader = Request.Headers["Authorization"];

            return base.HandleChallengeAsync(properties);
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //await Task.Delay(1);
            //return AuthenticateResult.NoResult();

            AuthenticateResult result = null;
            try
            {
                await Task.Delay(1);

                string authHeader = Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Bearer "))
                {
                    string accessToken = Guid.NewGuid().ToString();
                    // Get the token
                    var token = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                    // validatetoken
                    var handerJwt = new JwtSecurityTokenHandler();
                    var tokenInfo = handerJwt.ReadJwtToken(token);
                    SecurityToken validatedToken;
                    handerJwt.ValidateToken(token, new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Utils.GetConfig("Authentication:Jwt:Issuer"),
                        ValidAudience = Utils.GetConfig("Authentication:Jwt:Issuer"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Utils.GetConfig("Authentication:Jwt:Key")))
                    }, out validatedToken);
                    if (validatedToken != null
                    && validatedToken.Issuer == Utils.GetConfig("Authentication:Jwt:Issuer")
                    && validatedToken.ValidFrom.CompareTo(DateTime.Now) != 1
                    && validatedToken.ValidTo.CompareTo(DateTime.Now) != -1)
                    {
                        var claimsJwt = new List<Claim>();
                        var userId = tokenInfo.Claims.Where(x => x.Type == ClaimConstants.USER_ID).FirstOrDefault();
                        var roles = tokenInfo.Claims.Where(x => x.Type == ClaimConstants.ROLES).FirstOrDefault();
                        var level = tokenInfo.Claims.Where(x => x.Type == ClaimConstants.LEVEL).FirstOrDefault();
                        var userName = tokenInfo.Claims.Where(x => x.Type == ClaimConstants.USER_NAME).FirstOrDefault();
                        var fullName = tokenInfo.Claims.Where(x => x.Type == ClaimConstants.FULL_NAME).FirstOrDefault();

                        if (userId == null)
                            return AuthenticateResult.Fail("Invalidate Token");

                        claimsJwt.Add(new Claim(ClaimConstants.USER_ID, userId.ToString()));
                        claimsJwt.AddRange(tokenInfo.Claims);
                        ClaimsIdentity claimsIdentityJwt = new ClaimsIdentity(claimsJwt, "Jwt");
                        ClaimsPrincipal claimsPrincipalJwt = new ClaimsPrincipal(claimsIdentityJwt);

                        result = AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipalJwt,
                                            new AuthenticationProperties(), "Jwt"));

                        if (userId != null) Context.Items.Add(ClaimConstants.USER_ID, userId.Value);
                        if (level != null) Context.Items.Add(ClaimConstants.LEVEL, level.Value);
                        if (roles != null) Context.Items.Add(ClaimConstants.ROLES, roles.Value);
                        if (userName != null) Context.Items.Add(ClaimConstants.USER_NAME, userName.Value);
                        if (fullName != null) Context.Items.Add(ClaimConstants.FULL_NAME, fullName.Value);

                        return result;
                    }
                    else
                    {
                        return AuthenticateResult.Fail("Invalidate Token");
                    }
                }

                return AuthenticateResult.NoResult();
            }
            catch (Exception ex)
            {
                //return AuthenticateResult.
                Log.Debug(ex, "");
                return AuthenticateResult.NoResult();
                //return AuthenticateResult.Fail("Some error occurs. " + ex.Message);
            }
        }
    }
}