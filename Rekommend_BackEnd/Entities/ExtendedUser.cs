using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Entities
{
    public class ExtendedUser : AuditableEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        [Required]
        public Guid CompanyId { get; set; }
        [Required]
        public RecruiterPosition Position { get; set; }
        [Required]
        public DateTimeOffset DateOfBirth { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public SubscriptionPlan CurrentPlan { get;set; }
    }
}
