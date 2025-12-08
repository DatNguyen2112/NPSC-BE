using NSPC.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface IUserHandler
    {
        #region CRUD

        Task<Response> RegisterAsync(UserRegisterRequestModel model);

        Task<Response> ResetPassword(string resetPasswordToken, string password);

        Task<Response> VerifyEmail(string email, string token);

        Task<Response> CreateAsync(UserCreateModel model, Guid? tenantId);

        Task<Response> UpdateAsync(Guid id, UserUpdateModel model);

        Task<Response> UpdateInfoAsync(Guid id, UserInfoUpdateModel model);

        Task<Response> DeleteAsync(Guid id);

        #endregion CRUD

        Task<Response<bool>> CheckNameAvailability(string name);

        Task<Response<UserModel>> GetDetail(Guid id, string language);

        Task<Response<Pagination<UserModel>>> AdminGetPageAsync(UserQueryModel query);

        Task<Response> UpdateAvatarUrl(UserSetAvatarUrlModel model);

        Task<Response> ForceChangePassword(Guid id, string password);
        Task<Response> UpdateThemeConfig(ThemeConfigModel config, Guid userId);

        Task<Response> ChangePasswordAsync(UserUpdatePasswordModel model);

        Task<Response> ChangeLockStatusAsync(Guid id, bool status, Guid byUserId);
        Task<Response> ChangeActiveStatusAsync(Guid id, bool status, Guid byUserId);

        Task<Response> ForgotPassword(string email);
       Task<Response> VerifyForgotPasswordToken(string resetPasswordToken);

        #region Authentication
        Task<Response<LoginResponse>> Authentication(string userName, string password, bool isRememberMe, string deviceToken);

        Task<Response<LoginResponse>> BuildLoginResponse(Guid userId, bool isRememberMe);
        Task<Response<LoginResponse>> SwitchProfile(ProfileTypeModel model);
        Task<Response> RevokeToken(Guid id);
        Task<Response> UpdateLocation(UserLocationrUpdateModel model);
        Task<Response> SwitchStatus(UserStatusUpdateModel model);
        Task<Response> Logout(string isMobileDevice);
            #endregion

    }
}