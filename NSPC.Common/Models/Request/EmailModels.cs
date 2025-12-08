using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NSPC.Common.Models
{
    public class VerifyEmailModel
    {
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }

    public class ConfirmEmailModel
    {
        [StringLength(6)]
        public string Token { get; set; }
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [JsonIgnore]
        public Guid ByUserId { get; set; }
    }
}
