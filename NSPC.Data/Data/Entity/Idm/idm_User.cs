using NSPC.Data.Data.Entity.ChucVu;
using NSPC.Data.Data.Entity.NhomVatTu;
using NSPC.Data.Data.Entity.PhongBan;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace NSPC.Data
{
    [Table("idm_User")]
    public class idm_User : BaseTableService
    {
        public idm_User() { }

        [Key]
        public Guid Id { get; set; }
        public string? Ma { get; set; }
        [Required]
        [StringLength(128)]
        public string? UserName { get; set; }

        /*Attribute */
        [Required]
        [StringLength(128)]
        public string Name { get; set; }
        public Guid? IdChucVu { get; set; }
        [ForeignKey("IdChucVu")]
        public virtual mk_ChucVu mk_ChucVu { get; set; }
        public string MaPhongBan { get; set; }
        // [ForeignKey("IdPhongBan")]
        // public virtual mk_PhongBan mk_PhongBan { get; set; }
        public string MaTo { get; set; }
        [StringLength(32)]
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
        public string Gender { get; set; }
        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(1024)]
        public string AvatarUrl { get; set; }

        [StringLength(1024)]
        public string Password { get; set; }

        [StringLength(1024)]
        public string PasswordSalt { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime LastActivityDate { get; set; }
        [StringLength(128)]
        public string PlainTextPwd { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ActiveDate { get; set; }
        public int Level { get; set; }
        public string UpdateLog { get; set; }
        /* Social Fields */
        [StringLength(128)]
        public string FacebookUserId { get; set; }
        [StringLength(128)]
        public string GoogleUserId { get; set; }
        [StringLength(30)]
        public string ResetPasswordToken { get; set; }
        [StringLength(30)]
        public string EmailVerifyToken { get; set; }
        public bool IsEmailVerified { get; set; }
        public List<string> RoleListCode { get; set; }

        public string BankAccountNo { get; set; }
        public string BankName { get; set; }
        public string BankUsername { get; set; }
        public string StatusCode { get; set; }
        public string DeviceToken { get; set; }
        public string ThemeConfigs { get; set; }

    }
}