﻿using System;
using System.Collections.Generic;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Models
{
    public class TechJobOpeningDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ClosingDate { get; set; }
        public DateTimeOffset StartingDate { get; set; }
        public string Title { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCategory { get; set; }
        public Guid CreatedById { get; set; }
        public string RecruiterFirstName { get; set; }
        public string RecruiterLastName { get; set; }
        public string RecruiterPosition { get; set; }
        public string JobTechLanguage { get; set; }
        public string JobPosition { get; set; }
        public string Seniority { get; set; }
        public string ContractType { get; set; }
        public bool RemoteWorkAccepted { get; set; }
        public string MissionDescription { get; set; }
        public string City { get; set; }
        public int PostCode { get; set; }
        public string Country { get; set; }
        public string Reward { get; set; }
        public string BonusReward { get; set; }
        public int LikesNb { get; set; }
        public int RekommendationsNb { get; set; }
        public int ViewsNb { get; set; }
        public int MinimumSalary { get; set; }
        public int MaximumSalary { get; set; }
        public JobOfferStatus Status { get; set; }
        public string PictureFileName { get; set; }
        public string RseDescription { get; set; }
        public ICollection<RekommendationDto> Rekommendations { get; set; } = new List<RekommendationDto>();
    }
}
