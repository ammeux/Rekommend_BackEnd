using System;
using System.ComponentModel.DataAnnotations;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Entities
{
    public class Company
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public DateTimeOffset RegistrationDate { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string HqCity { get; set; }
        [Required]
        [MaxLength(1500)]
        public string CompanyDescription { get; set; }
        [Required]
        [MaxLength(50)]
        public CompanyCategory Category { get; set; }
        [Required]
        [MaxLength(50)]
        public string LogoFileName { get; set; }
        [Required]
        [MaxLength(50)]
        public string Website { get; set; }
    }
}
