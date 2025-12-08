using NSPC.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NSPC.Business.Services.NhomVatTu;
using NSPC.Business.Services.ChucVu;
using NSPC.Business.Services.PhongBan;

namespace NSPC.Business
{
    public class BaseUserListModel
    {
        public Guid Id { get; set; }
        public string? Ma { get; set; }
        public string? UserName { get; set; }
        public string Name { get; set; }
        public ChucVuViewModel ChucVu { get; set; }
        public PhongBanViewModel PhongBan { get; set; }
        public string Team { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        //public DateTime? LocalTime
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(this.TimeZone))
        //            return DateTime.UtcNow;
        //        else
        //        {
        //            var timeZone = TimezoneCollection.Instance.AllTimeZones.Find(x => x.Abbr.ToLower() == this.TimeZone.ToLower());
        //            if (timeZone != null)
        //                return DateTime.UtcNow.AddHours(timeZone.Offset);
        //            else return DateTime.UtcNow;
        //        }
        //    }
        //}
        [JsonIgnore]
        public string TimeZone { get; set; }
    }
    public class BaseUserModel
    {
        public Guid Id { get; set; }
        public string? Ma { get; set; }
        public string? UserName { get; set; }
        public string Name { get; set; }
        public string MaPhongBan { get; set; }
        public string MaTo { get; set; }
        public ChucVuViewModel ChucVu { get; set; }
        public CodeTypeListModel PhongBan { get; set; }
        public CodeTypeItemViewModel ToThucHien { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }
        public string Gender { get; set; }
        public string GenderString { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public string BankAccountNo { get; set; }
        public string BankName { get; set; }
        public string BankUsername { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        
        [JsonIgnore]
        public string PasswordSalt { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime LastActivityDate { get; set; }
        [JsonIgnore]
        public string PlainTextPwd { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ActiveDate { get; set; }
        public int Level { get; set; }
        [JsonIgnore]
        public string UpdateLog { get; set; }
        public string FacebookUserId { get; set; }
        public string GoogleUserId { get; set; }
        [JsonIgnore]
        public string ResetPasswordToken { get; set; }
        public string EmailVerifyToken { get; set; }
        public bool IsEmailVerified { get; set; }
        public List<string> RoleListCode { get; set; }
        // public string ProfileType { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }



    public class UserModel : BaseUserModel
    {
        public List<BaseRoleModel> ListRole { get; set; }
        public BaseRoleModel Role { get; set; }
        public string[] AllowedActions { get; set; } = Array.Empty<string>();
        public int TotalClinic { get; set; }
        public int TotalOrder { get; set; }
        public Guid? ProfileFarmerId { get; set; }
    }

    public class UserQueryModel : PaginationRequest
    {
        public bool? IsLockedOut { get; set; }
        public int? Level { get; set; }
        public int? Type { get; set; }
        public string? UserName { get; set; }
        public string RoleCode { get; set; }
        public int? ToPoint { get; set; }
        public Guid? PartnerId { get; set; }
        public Guid? UserId { get; set; }
        public bool? IsEmployee { get; set; } = false;
        public DateTime?[] DateRange { get; set; }
        public bool? IsActive { get; set; }
        public string ProfileType { get; set; }
        public bool? IsEmailVerified { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? DepartmentId { get; set; }
        public string[] RoleListCode { get; set; }
    }

    public class UserCreateModel
    {
        public string? Ma { get; set; }
        public string? UserName { get; set; }
        public string Name { get; set; }
        public Guid? IdChucVu { get; set; }
        public string MaPhongBan { get; set; }
        public string MaTo { get; set; }
        [RegularExpression("[0-9]{10}", ErrorMessage = "Số điện thoại gồm 10 số")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string PhoneNumber { get; set; }
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Email không hợp lệ")]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }
        public DateTime? Birthdate { get; set; }
        public bool? IsLockedOut { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsRevokedToken { get; set; }
        public List<string> RoleListCode { get; set; }
        public string Gender { get; set; }
    }

    public class UserRegisterRequestModel
    {
        [EmailAddress(ErrorMessage ="Email không hợp lệ")]
        public string Email { get; set; }
        public string Name { get; set; }
        public Guid IdChucVu { get; set; }
        public string MaPhongBan  { get; set; }
        public string MaTo { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
        public string MedicalDegreeCode { get; set; }
        [RegularExpression("[0-9]{10}", ErrorMessage = "Số điện thoại gồm 10 số")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại", AllowEmptyStrings = true)]
        public string PhoneNumber { get; set; }

        [JsonIgnore]
        public string Currency { get; set; }
        [JsonIgnore]
        public string Language { get; set; }

        [JsonIgnore]
        public string AvatarUrl { get; set; }

        [JsonIgnore]
        public string FacebookUserId { get; set; }

        [JsonIgnore]
        public string GoogleUserId { get; set; }

        /// <summary>
        /// Pre-defined user Id, used for seeding accounts
        /// </summary>
        [JsonIgnore]
        public Guid? UserId { get; set; }
    }

    public class UserUpdateModel
    {
        public string? Ma { get; set; }
        public string? UserName { get; set; }
        public string Name { get; set; }
        public Guid? IdChucVu { get; set; }
        public string MaPhongBan  { get; set; }
        public string MaTo { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? Birthdate { get; set; }
        public bool? IsLockedOut { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsRevokedToken { get; set; }
        public List<string> RoleListCode { get; set; }
        public string Gender { get; set; }
    }

    public class UserFeedBackVIew
    {
        public string? UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        
    }

    public class UserInfoUpdateModel
    {
        [Required(ErrorMessage = "Vui lòng nhập têni", AllowEmptyStrings = true)]
        public string Name { get; set; }
        public Guid IdChucVu { get; set; }
        public Guid? IdPhongBan { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại", AllowEmptyStrings = true)]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Số điện thoại gồm 10 số")]
        public string PhoneNumber { get; set; }
        [RegularExpression("[0-9]*",ErrorMessage ="Tài khoản ngân hàng chỉ nhập số")]
        [Required(ErrorMessage = "Vui lòng nhập số tài khoản", AllowEmptyStrings = true)]
        public string BankAccountNo { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập tên ngân hàng", AllowEmptyStrings =true)]
        [RegularExpression("[a-zA-Z ]*", ErrorMessage = "Tên ngân hàng không được nhập số hoặc ký tự")]
        public string BankName { get; set; }
        [JsonIgnore]
        public string GtsNumber { get; set; }
        [JsonIgnore]
        public string IdentificationNumber { get; set; }
        [JsonIgnore]
        public string CountryCode { get; set; }
        [JsonIgnore]
        public string TimeZone { get; set; }
        [JsonIgnore]
        public string Currency { get; set; }
        [JsonIgnore]
        public string Language { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản ngân hàng ", AllowEmptyStrings = true)]
        public string BankUsername { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn giới tính", AllowEmptyStrings = true)]
        public string Gender { get; set; }
    }

    public class CapUserLevelUpdateModel
    {
        public Guid Id { get; set; }
        public int Level { get; set; }
    }

    public class CapUserInfoUpdateModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int? Level { get; set; }
        public bool? IsLockedOut { get; set; }
        public string ProfileType { get; set; }
    }

    public class CapUserLockedUpdateModel
    {
        public Guid Id { get; set; }
        public bool Locked { get; set; }
    }

    public class UserResetPasswordModel
    {
        public string ResetPasswordToken { get; set; }
        public string Password { get; set; }
    }

    public class UserPasswordModel
    {
        public string Password { get; set; }
    }


    public class UserVerifyEmailModel
    {
        public string Token { get; set; }
        public string Email { get; set; }
    }

    public class UserUpdatePasswordModel
    {
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class UserSetAvatarUrlModel
    {
        public string AvatarUrl { get; set; }
        public Guid UserId { get; set; }
    }

    public class ProfileTypeModel
    {
        public string ProfileType { get; set; }
    }    

    public class UserLocationrUpdateModel
    {
        public double Lat { get; set; }
        public double Long { get; set; }
    }
    public class UserStatusUpdateModel
    {
        public string StatusCode { get; set; }
    }

    public class VerifyForgotPasswordTokenModel
    {
        public string ResetPasswordToken { get; set; }
    }
}