using System.ComponentModel.DataAnnotations;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Models
{
    public class CompanyForManipulationAbstract
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string HqCity { get; set; }
        [Required]
        [MaxLength(50)]
        public string HqCountry { get; set; }
        [Required]
        [MaxLength(1500)]
        public string CompanyDescription { get; set; }
        [Required]
        [MaxLength(50)]
        public string Category { get; set; }
        [Required]
        [MaxLength(50)]
        public string LogoFileName { get; set; }
        [Required]
        [MaxLength(50)]
        public string Website { get; set; }
        [Required]
        public int PostCode { get; set; }
    }
}
