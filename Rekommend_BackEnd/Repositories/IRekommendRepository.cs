using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.ResourceParameters;
using Rekommend_BackEnd.Utils;
using System;

namespace Rekommend_BackEnd.Repositories
{
    public interface IRekommendRepository
    {
        // TechJobOpening
        TechJobOpening GetTechJobOpening(Guid techJobOpeningId);
        PagedList<TechJobOpening> GetTechJobOpenings(TechJobOpeningsResourceParameters techJobOpeningsResourceParameters);
        void AddTechJobOpening(Guid recruiterId, TechJobOpening techJobOpening);
        void UpdateTechJobOpening(TechJobOpening techJobOpening);
        void DeleteTechJobOpening(TechJobOpening techJobOpening);
        bool IsAuthorizedToPublish(Guid recruiterId);

        // Recruiter
        Recruiter GetRecruiter(Guid recruiterId);
        PagedList<Recruiter> GetRecruiters(RecruitersResourceParameters recruitersResourceParameters);
        void AddRecruiter(Guid companyId, Recruiter recruiter);
        void UpdateRecruiter(Recruiter recruiter);
        void DeleteRecruiter(Recruiter recruiter);

        // Company
        Company GetCompany(Guid companyId);
        PagedList<Company> GetCompanies(CompaniesResourceParameters companiesResourceParameters);
        void AddCompany(Company company);
        void UpdateCompany(Company company);
        void DeleteCompany(Company company);

        // Rekommendation
        Rekommendation GetRekommendation(Guid rekommendationId);
        PagedList<Rekommendation> GetRekommendations(RekommendationsResourceParameters rekommendationsResourceParameters);
        void AddRekommendation(Guid rekommenderId, Rekommendation rekommendation);
        void UpdateRekommendation(Rekommendation rekommendation);
        void DeleteRekommendation(Rekommendation rekommendation);

        // Rekommender
        Rekommender GetRekommender(Guid RekommenderId);
        void RecomputeXpAndRekoAvgFromRekommender(Guid rekommenderId);
        PagedList<Rekommender> GetRekommenders(RekommendersResourceParameters rekommendersResourceParameters);
        void AddRekommender(Rekommender rekommender);
        void UpdateRekommender(Rekommender rekommender);
        void DeleteRekommender(Rekommender rekommender);

        bool Save();
    }
}
