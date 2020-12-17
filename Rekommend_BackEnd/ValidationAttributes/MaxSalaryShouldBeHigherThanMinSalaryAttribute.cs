using Rekommend_BackEnd.Models;
using System.ComponentModel.DataAnnotations;

namespace Rekommend_BackEnd.ValidationAttributes
{
    public class MaxSalaryShouldBeHigherThanMinSalaryAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var techJobOpening = (TechJobOpeningForManipulationAbstract)validationContext.ObjectInstance;

            if(techJobOpening.MinimumSalary != 0 && techJobOpening.MaximumSalary !=0 && techJobOpening.MinimumSalary>techJobOpening.MaximumSalary)
            {
                return new ValidationResult("Minimum salary cannot be higher than the maximum salary.", new[] { nameof(TechJobOpeningForManipulationAbstract) });
            }

            return ValidationResult.Success;
        }
    }
}
