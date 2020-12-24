using System;
using System.ComponentModel.DataAnnotations;

namespace Rekommend_BackEnd.Models
{
    public abstract class RekommendationForManipulationAbstract
    {
        [Required]
        public Guid TechJobOpeningId { get; set; }
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
        public string Email { get; set; }
        [Required]
        [MaxLength(1500)]
        public string Comment { get; set; }
        [Required]
        public bool HasAlreadyWorkedWithRekommender { get; set; }
        public int Grade { get; set; }
    }
}
