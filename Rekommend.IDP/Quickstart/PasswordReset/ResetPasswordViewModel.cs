﻿using System.ComponentModel.DataAnnotations;

namespace Rekommend.IDP.PasswordReset
{
    public class ResetPasswordViewModel
    {
        [Required]
        [MaxLength(200)]
        [DataType(DataType.Password)]
        [Display(Name = "Your new password")]
        public string Password { get; set; }

        [Required]
        [MaxLength(200)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm your new password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string SecurityCode { get; set; }
    }
}
