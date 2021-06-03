using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Rekommend.IDP.UserRegistration
{
    public class RegisterUserFromFacebookViewModel
    {
        [Required]
        [MaxLength(250)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Given name")]
        public string GivenName { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Family name")]
        public string FamilyName { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [MaxLength(2)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        public string Email { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Company")]
        public string Company { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Profile")]
        public string Profile { get; set; }

        [MaxLength(200)]
        [Display(Name = "Stack")]
        public string Stack { get; set; }

        [MaxLength(200)]
        [Display(Name = "Seniority")]
        public string Seniority { get; set; }

        public SelectList ProfileList { get; set; } =
            new SelectList(
                new[]
                {
                    new { Id = "Tech", Value = "Tech"},
                    new { Id = "Non-Tech", Value = "Non-Tech"}
                },
                "Id",
                "Value");

        [Display(Name = "Stack")]
        public SelectList StackList { get; set; } =
            new SelectList(
                new[]
                {
                    new {Id = "Undefined", Value = "Non applicable"},
                    new {Id = "Javascript", Value = "Javascript"},
                    new {Id = "Python", Value = "Python"},
                    new {Id = "CS", Value = "C#"},
                    new {Id = "Java", Value = "Java"}
                },
                "Id",
                "Value");

        public SelectList SeniorityList { get; set; } =
            new SelectList(
                new[]
                {
                    new{Id = "Undefined", Value = "Non applicable"},
                    new{Id = "Junior", Value = "Junior"},
                    new{Id = "Mid-Senior", Value = "Mid-Senior"},
                    new{Id = "Senior", Value = "Senior"},
                    new{Id = "Expert", Value = "Expert"}
                },
                "Id",
                "Value");
        public string ReturnUrl { get; set; }
        public SelectList CountryCodes { get; set; } =
           new SelectList(
               new[] {
                    new { Id = "FR", Value = "France" },
                    new { Id = "CA", Value = "Canada" },
                    new { Id = "BE", Value = "Belgium" } },
               "Id",
               "Value");

        public string Provider { get; set; }
        public string ProviderUserId { get; set; }


    }
}
