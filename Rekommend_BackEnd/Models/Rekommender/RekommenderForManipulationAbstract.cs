using System;
using System.ComponentModel.DataAnnotations;

namespace Rekommend_BackEnd.Models
{
    public abstract class RekommenderForManipulationAbstract
    {
        [Required]
        public DateTimeOffset DateOfBirth { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public string Seniority { get; set; }
        [Required]
        [MaxLength(50)]
        public string Company { get; set; }
        [Required]
        [MaxLength(50)]
        public string City { get; set; }
        [Required]
        [MaxLength(50)]
        public string Email { get; set; }
        [Required]
        public int PostCode { get; set; }
    }
}
