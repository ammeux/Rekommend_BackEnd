using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Entities
{
    public class TechJobOpening : AuditableEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTimeOffset ClosingDate { get; set; }
        public DateTimeOffset StartingDate { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        [Required]
        public Guid CompanyId { get; set; }
        [Required]
        [MaxLength(50)]
        public string CreatedByFirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string CreatedByLastName { get; set; }
        [Required]
        [MaxLength(50)]
        public RecruiterPosition CreatedByPosiion { get; set; }
        [Required]
        public JobTechLanguage JobTechLanguage { get; set; }
        [Required]
        public Position JobPosition { get; set; }
        [Required]
        public Seniority Seniority { get; set; }
        [Required]
        public ContractType ContractType { get; set; }
        [Required]
        public bool RemoteWorkAccepted { get; set; }
        [Required]
        [MaxLength(1500)]
        public string MissionDescription { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public int PostCode { get; set; }
        [Required]
        public Country Country { get; set; }
        [MaxLength(50)]
        public string Reward { get; set; }
        [MaxLength(50)]
        public string BonusReward { get; set; }
        public int LikesNb { get; set; }
        public int RekommendationsNb { get; set; }
        public int ViewsNb { get; set; }
        public int MinimumSalary { get; set; }
        public int MaximumSalary { get; set; }
        [Required]
        public JobOfferStatus Status { get; set; }
        [MaxLength(50)]
        public string PictureFileName { get; set; }
        [MaxLength(1500)]
        public string RseDescription { get; set; }
    }
}
