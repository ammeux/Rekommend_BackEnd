using System.ComponentModel.DataAnnotations;

namespace Rekommend.IDP.PasswordReset
{
    public class RequestPasswordViewModel
    {
        [Required]
        [MaxLength(200)]
        [Display(Name="email")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
