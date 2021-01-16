using PagedList;
using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.ResourceParameters;
using System;
using System.Threading.Tasks;

namespace Rekommend_BackEnd.Repositories
{
    public interface IRekommendRepository
    {
        // TechJobOpening
        Task<TechJobOpening> GetTechJobOpeningAsync(Guid techJobOpeningId);
        Task<IPagedList<TechJobOpening>> GetTechJobOpeningsAsync(TechJobOpeningsResourceParameters techJobOpeningsResourceParameters);
        void AddTechJobOpening(Guid recruiterId, TechJobOpening techJobOpening);
        void UpdateTechJobOpening(TechJobOpening techJobOpening);
        void DeleteTechJobOpening(TechJobOpening techJobOpening);
        bool IsAuthorizedToPublish(Guid recruiterId);

        // Recruiter
        Task<Recruiter> GetRecruiterAsync(Guid recruiterId);
        Task<IPagedList<Recruiter>> GetRecruitersAsync(RecruitersResourceParameters recruitersResourceParameters);
        void AddRecruiter(Guid companyId, Recruiter recruiter);
        void UpdateRecruiter(Recruiter recruiter);
        void DeleteRecruiter(Recruiter recruiter);

        // Company
        Task<Company> GetCompanyAsync(Guid companyId);
        Task<IPagedList<Company>> GetCompaniesAsync(CompaniesResourceParameters companiesResourceParameters);
        void AddCompany(Company company);
        void UpdateCompany(Company company);
        void DeleteCompany(Company company);

        // Rekommendation
        Task<Rekommendation> GetRekommendationAsync(Guid rekommendationId);
        Task<IPagedList<Rekommendation>> GetRekommendationsAsync(RekommendationsResourceParameters rekommendationsResourceParameters);
        void AddRekommendation(Guid rekommenderId, Rekommendation rekommendation);
        void UpdateRekommendation(Rekommendation rekommendation);
        void DeleteRekommendation(Rekommendation rekommendation);

        // Rekommender
        Task<Rekommender> GetRekommenderAsync(Guid RekommenderId);
        void RecomputeXpAndRekoAvgFromRekommender(Guid rekommenderId);
        Task<IPagedList<Rekommender>> GetRekommendersAsync(RekommendersResourceParameters rekommendersResourceParameters);
        void AddRekommender(Rekommender rekommender);
        void UpdateRekommender(Rekommender rekommender);
        void DeleteRekommender(Rekommender rekommender);

        Task<bool> SaveChangesAsync();
    }
}
