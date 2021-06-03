using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Entities
{
    public class Rekommendation : AuditableEntity
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("TechJobOpeningId")]
        public TechJobOpening TechJobOpening { get; set; }
        [Required]
        public Guid TechJobOpeningId { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        public Position Position { get; set; }
        [Required]
        public Seniority Seniority { get; set; }
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
        public RekommendationStatus Status { get; set; }
        [Required]
        public bool HasAlreadyWorkedWithRekommender { get; set; }
        [Required]
        public int Grade { get; set; }
    }
}
