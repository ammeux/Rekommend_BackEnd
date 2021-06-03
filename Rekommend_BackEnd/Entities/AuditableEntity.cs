using System;
using System.ComponentModel.DataAnnotations;

namespace Rekommend_BackEnd.Entities
{
    public class AuditableEntity
    {
        [Required]
        public DateTimeOffset CreatedOn { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        [Required]
        public DateTimeOffset UpdatedOn { get; set; }

        [Required]
        public Guid UpdatedBy { get; set; }
    }
}
