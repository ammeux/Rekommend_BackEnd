using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Entities
{
    public class JobOpening
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset ClosingDate { get; set; }
        public DateTimeOffset StartingDate { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required]
        public Guid RecruiterId { get; set; }
        [ForeignKey("RecruiterId")]
        public Recruiter Recruiter { get; set; }
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
        [MaxLength(50)]
        public string City { get; set; }
        [Required]
        [MaxLength(50)]
        public string Country { get; set; }
        [MaxLength(50)]
        public string Reward1 { get; set; }
        [MaxLength(50)]
        public string Reward2 { get; set; }
        [MaxLength(50)]
        public string Reward3 { get; set; }
        public int LikesNb { get; set; }
        public int RekommendationsNb { get; set; }
        public int ViewNb { get; set; }
        public int MinimumSalary { get; set; }
        public int MaximumSalary { get; set; }
        [Required]
        public JobOfferStatus Status { get; set; }
        [MaxLength(50)]
        public string pictureFileName { get; set; }
        [MaxLength(1500)]
        public string RseDescription { get; set; }
    }
}
