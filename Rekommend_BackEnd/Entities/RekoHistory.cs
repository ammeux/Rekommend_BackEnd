using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Entities
{
    public class RekoHistory
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("RekommenderId")]
        public Rekommender Rekommender { get; set; }
        [Required]
        public Guid RekommenderId { get; set; }
        [ForeignKey("RekommendationId")]
        public Rekommendation Rekommendation { get; set; }
        [Required]
        public Guid RekommendationId { get; set; }
        [ForeignKey("TechJobOpeningId")]
        public TechJobOpening TechJobOpeing { get; set; }
        [Required]
        public Guid TechJobOpeningId { get; set; }
        [Required]
        public DateTimeOffset Date { get; set; }
        [Required]
        public RekommendationStatus RekommendationStatus { get; set; }
    }
}
