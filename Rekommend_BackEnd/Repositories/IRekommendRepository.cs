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
        bool IsAuthorizedToPublish(Guid userId);

        // Recruiter
        Task<ExtendedUser> GetExtendedUserAsync(Guid extendedUserId);
        Task<IPagedList<ExtendedUser>> GetExtendedUsersAsync(ExtendedUsersResourceParameters recruitersResourceParameters);
        void AddExtendedUser(Guid companyId, ExtendedUser extendedUser);
        void UpdateExtendedUser(ExtendedUser extendedUser);
        void DeleteExtendedUser(ExtendedUser extendedUser);

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

        Task<bool> SaveChangesAsync();
    }
}
