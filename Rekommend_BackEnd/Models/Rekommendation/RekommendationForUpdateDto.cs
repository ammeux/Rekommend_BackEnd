using System.ComponentModel.DataAnnotations;

namespace Rekommend_BackEnd.Models
{
    public class RekommendationForUpdateDto : RekommendationForManipulationAbstract
    {
        [MaxLength(50)]
        public string RekommendationStatus { get; set; }
    }
}
