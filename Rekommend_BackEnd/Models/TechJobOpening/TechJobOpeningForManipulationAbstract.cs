using Rekommend_BackEnd.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rekommend_BackEnd.Models
{
    [MaxSalaryShouldBeHigherThanMinSalaryAttribute]
    public abstract class TechJobOpeningForManipulationAbstract : IValidatableObject
    {
        public DateTimeOffset StartingDate { get; set; }
        [Required(ErrorMessage = "Title is required to create your job opening")]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required]
        public string JobTechLanguage { get; set; }
        [Required]
        public string JobPosition { get; set; }
        [Required]
        public string Seniority { get; set; }
        [Required]
        public string ContractType { get; set; }
        [Required]
        public bool RemoteWorkAccepted { get; set; }
        [Required]
        [MaxLength(1500)]
        public string MissionDescription { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public int PostCode { get; set; }
        [Required]
        public string Country { get; set; }
        [MaxLength(50)]
        public string Reward1 { get; set; }
        [MaxLength(50)]
        public string Reward2 { get; set; }
        [MaxLength(50)]
        public string Reward3 { get; set; }
        public int MinimumSalary { get; set; }
        public int MaximumSalary { get; set; }
        [MaxLength(50)]
        public string PictureFileName { get; set; }
        [MaxLength(1500)]
        public string RseDescription { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == MissionDescription)
            {
                yield return new ValidationResult("The provided description should be different from the title.", new[] { "TechJobOpeningForCreationDto" });
            }
        }
    }
}
