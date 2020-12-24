using System;
using System.ComponentModel.DataAnnotations;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Models
{
    public abstract class RecruiterForManipulationAbstract
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public DateTimeOffset DateOfBirth { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public Gender Gender { get; set; }
    }
}
