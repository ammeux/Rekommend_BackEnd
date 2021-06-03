using Microsoft.EntityFrameworkCore;
using PagedList;
using Rekommend_BackEnd.DbContexts;
using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Extensions;
using Rekommend_BackEnd.Models;
using Rekommend_BackEnd.ResourceParameters;
using Rekommend_BackEnd.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Repositories
{
    public class RekommendRepository : IRekommendRepository, IDisposable
    {
        private readonly ApplicationContext _context;
        private readonly IPropertyMappingService _propertyMappingService;
        //private readonly IEntityPropertiesService _entityPropertiesService;

        public RekommendRepository(ApplicationContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            //_entityPropertiesService = entityPropertiesService ?? throw new ArgumentNullException(nameof(entityPropertiesService));
        }

        public IQueryable<TechJobOpening> GetTechJobOpenings()
        {
            return _context.TechJobOpenings.Include(d => d.Company) as IQueryable<TechJobOpening>;
        }

        public async Task<IPagedList<TechJobOpening>> GetTechJobOpeningsAsync(TechJobOpeningsResourceParameters techJobOpeningsResourceParameters)
        {
            if (techJobOpeningsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(techJobOpeningsResourceParameters));
            }

            var collection = GetTechJobOpenings();

            var companyCategory = techJobOpeningsResourceParameters.CompanyCategory.ToCompanyCategory();
            if (companyCategory != CompanyCategory.Undefined)
            {
                collection = collection.Where(a => a.Company.Category == companyCategory);
            }

            var jobTechLanguage = techJobOpeningsResourceParameters.JobTechLanguage.ToJobTechLanguage();
            if (jobTechLanguage != JobTechLanguage.Undefined)
            {
                collection = collection.Where(a => a.JobTechLanguage == jobTechLanguage);
            }

            var city = techJobOpeningsResourceParameters.City;
            if (city != null)
            {
                collection = collection.Where(a => a.City == city);
            }

            var postCode = techJobOpeningsResourceParameters.PostCode;
            if(Math.Floor(Math.Log10(postCode) + 1) == 5)
            {
                collection = collection.Where(a => a.PostCode == postCode);
            }

            if(techJobOpeningsResourceParameters.RemoteWorkAccepted == true)
            {
                collection = collection.Where(a => a.RemoteWorkAccepted == true);
            }

            var contractType = techJobOpeningsResourceParameters.ContractType.ToContractType();
            if(contractType != ContractType.Undefined)
            {
                collection = collection.Where(a => a.ContractType == contractType);
            }

            var jobPosition = techJobOpeningsResourceParameters.JobPosition.ToPosition();
            if(jobPosition != Position.Undefined)
            {
                collection = collection.Where(a => a.JobPosition == jobPosition);
            }

            var seniority = techJobOpeningsResourceParameters.Seniority.ToSeniority();
            if(seniority != Seniority.Undefined)
            {
                collection = collection.Where(a => a.Seniority == seniority);
            }

            if(!string.IsNullOrWhiteSpace(techJobOpeningsResourceParameters.SearchQuery))
            {
                var searchQuery = techJobOpeningsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Company.Name.ToLower().Contains(searchQuery.ToLower()));
            }

            if(!string.IsNullOrWhiteSpace(techJobOpeningsResourceParameters.OrderBy))
            {
                // get property mapping dictionary
                var techJobOpeningMappingDictionary = _propertyMappingService.GetPropertyMapping<TechJobOpeningDto, TechJobOpening>();

                collection = collection.ApplySort(techJobOpeningsResourceParameters.OrderBy, techJobOpeningMappingDictionary);
            }

            return await Utils.PagedList<TechJobOpening>.Create(collection, techJobOpeningsResourceParameters.PageNumber, techJobOpeningsResourceParameters.PageSize);
        }

        //public IDictionary<Guid, Recruiter> GetRecruiters(IEnumerable<Guid> recruiterIds)
        //{
        //    if(recruiterIds == null)
        //    {
        //        throw new ArgumentNullException(nameof(recruiterIds));
        //    }

        //    return _context.Recruiters.Where(a => recruiterIds.Contains(a.Id)).ToDictionary(x=>x.Id, y=>y);
        //}

        public async Task<IPagedList<ExtendedUser>> GetExtendedUsersAsync(ExtendedUsersResourceParameters recruitersResourceParameters)
        {
            if (recruitersResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(recruitersResourceParameters));
            }

            var collection = _context.Recruiters as IQueryable<ExtendedUser>;

            var recruiterPosition = recruitersResourceParameters.RecruiterPosition.ToRecruiterPosition();
            if(recruiterPosition != RecruiterPosition.Undefined)
            {
                collection = collection.Where(a => a.Position == recruiterPosition);
            }

            var companyId = recruitersResourceParameters.CompanyId;
            if(companyId != null)
            {
                collection = collection.Where(a => a.CompanyId.ToString() == companyId);
            }

            if (!string.IsNullOrWhiteSpace(recruitersResourceParameters.SearchQuery))
            {
                var searchQuery = recruitersResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Company.Name.ToLower().Contains(searchQuery.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(recruitersResourceParameters.OrderBy))
            {
                // get property mapping dictionary
                var recruiterMappingDictionary = _propertyMappingService.GetPropertyMapping<ExtendedUserDto, ExtendedUser>();

                collection = collection.ApplySort(recruitersResourceParameters.OrderBy, recruiterMappingDictionary);
            }

            return await Utils.PagedList<ExtendedUser>.Create(collection, recruitersResourceParameters.PageNumber, recruitersResourceParameters.PageSize);
        }

        //public IEnumerable<Recruiter> GetCompanies(IEnumerable<Guid> companyIds)
        //{
        //    if (companyIds == null)
        //    {
        //        throw new ArgumentNullException(nameof(companyIds));
        //    }

        //    return _context.Recruiters.Where(a => companyIds.Contains(a.Id));
        //}

        public async Task<IPagedList<Company>> GetCompaniesAsync(CompaniesResourceParameters companiesResourceParameters)
        {
            if (companiesResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(companiesResourceParameters));
            }

            var collection = _context.Companies as IQueryable<Company>;

            var name = companiesResourceParameters.Name;
            if (name != null)
            {
                collection = collection.Where(a => a.Name == name);
            }

            var hqCity = companiesResourceParameters.HqCity;
            if (hqCity != null)
            {
                collection = collection.Where(a => a.HqCity == hqCity);
            }

            var hqCountry = companiesResourceParameters.HqCountry;
            if (hqCountry != null)
            {
                collection = collection.Where(a => a.HqCountry == hqCountry);
            }

            var category = companiesResourceParameters.Category;
            if (category != null)
            {
                collection = collection.Where(a => a.Category.ToString() == category);
            }

            if (!string.IsNullOrWhiteSpace(companiesResourceParameters.SearchQuery))
            {
                var searchQuery = companiesResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Name.ToLower().Contains(searchQuery.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(companiesResourceParameters.OrderBy))
            {
                // get property mapping dictionary
                var companyMappingDictionary = _propertyMappingService.GetPropertyMapping<CompanyDto, Company>();

                collection = collection.ApplySort(companiesResourceParameters.OrderBy, companyMappingDictionary);
            }

            return await Utils.PagedList<Company>.Create(collection, companiesResourceParameters.PageNumber, companiesResourceParameters.PageSize);
        }

        public async Task<ExtendedUser> GetExtendedUserAsync(Guid recruiterId)
        {
            if (recruiterId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(recruiterId));
            }

            return await _context.Recruiters.Include(d => d.Company).FirstOrDefaultAsync(a => a.UserId == recruiterId);
        }

        public void AddExtendedUser(Guid companyId, ExtendedUser recruiter)
        {
            var company = _context.Companies.FirstOrDefault(a => a.Id == companyId);
            if(companyId == null || company == null)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            if(recruiter == null)
            {
                throw new ArgumentNullException(nameof(recruiter));
            }

            recruiter.Id = Guid.NewGuid();
            recruiter.CreatedOn = DateTimeOffset.Now;
            recruiter.CompanyId = companyId;
            _context.Recruiters.Add(recruiter);
        }

        public async Task<Company> GetCompanyAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            return await _context.Companies.FirstOrDefaultAsync(a => a.Id == companyId);
        }

        public void AddCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            company.Id = Guid.NewGuid();
            company.CreatedOn = DateTimeOffset.Now;
            _context.Companies.Add(company);
        }

        public async Task<TechJobOpening> GetTechJobOpeningAsync(Guid techJobOpeningId)
        {
            if (techJobOpeningId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(techJobOpeningId));
            }

            return await _context.TechJobOpenings.Include(d => d.CreatedBy).Include(d => d.Company).FirstOrDefaultAsync(a => a.Id == techJobOpeningId);
        }

        public void AddTechJobOpening(Guid recruiterId, TechJobOpening techJobOpening)
        {
            if(recruiterId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(recruiterId));
            }

            if(techJobOpening == null)
            {
                throw new ArgumentNullException(nameof(techJobOpening));
            }

            techJobOpening.Id = Guid.NewGuid();
            techJobOpening.CreatedOn = DateTimeOffset.Now;
            techJobOpening.CreatedBy = recruiterId;
            techJobOpening.UpdatedOn = DateTimeOffset.Now;
            techJobOpening.UpdatedBy = recruiterId;
            _context.TechJobOpenings.Add(techJobOpening);
        }

        public async Task<Rekommendation> GetRekommendationAsync(Guid rekommendationId)
        {
            if (rekommendationId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(rekommendationId));
            }

            return await _context.Rekommendations.Include(d => d.TechJobOpening).FirstOrDefaultAsync(a => a.Id == rekommendationId);
        }

        public async Task<IPagedList<Rekommendation>> GetRekommendationsAsync(RekommendationsResourceParameters rekommendationsResourceParameters)
        {
            if (rekommendationsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(rekommendationsResourceParameters));
            }

            var collection = _context.Rekommendations.Include(d=>d.TechJobOpening) as IQueryable<Rekommendation>;

            var techJobOpeningId = rekommendationsResourceParameters.TechJobOpeningId;
            if (techJobOpeningId != null)
            {
                collection = collection.Where(a => a.TechJobOpening.Id.ToString() == techJobOpeningId);
            }

            var position = rekommendationsResourceParameters.Position;
            if (position != null)
            {
                collection = collection.Where(a => a.Position.ToString() == position);
            }

            var seniority = rekommendationsResourceParameters.Seniority;
            if (seniority != null)
            {
                collection = collection.Where(a => a.Seniority.ToString() == seniority);
            }

            var rekommendationStatus = rekommendationsResourceParameters.Status;
            if (rekommendationStatus != null)
            {
                collection = collection.Where(a => a.Status.ToString() == rekommendationStatus);
            }

            var hasAlreadyWorkedWithRekommender = rekommendationsResourceParameters.HasAlreadyWorkedWithRekommender;
            if(hasAlreadyWorkedWithRekommender == true)
            {
                collection = collection.Where(a => a.HasAlreadyWorkedWithRekommender);
            }

            if (!string.IsNullOrWhiteSpace(rekommendationsResourceParameters.SearchQuery))
            {
                var searchQuery = rekommendationsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.LastName.ToLower().Contains(searchQuery.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(rekommendationsResourceParameters.OrderBy))
            {
                // get property mapping dictionary
                var rekommendationMappingDictionary = _propertyMappingService.GetPropertyMapping<RekommendationDto, Rekommendation>();

                collection = collection.ApplySort(rekommendationsResourceParameters.OrderBy, rekommendationMappingDictionary);
            }

            return await Utils.PagedList<Rekommendation>.Create(collection, rekommendationsResourceParameters.PageNumber, rekommendationsResourceParameters.PageSize);
        }

        public void AddRekommendation(Guid rekommenderId, Rekommendation rekommendation)
        {
            if (rekommendation == null)
            {
                throw new ArgumentNullException(nameof(rekommendation));
            }

            rekommendation.Id = Guid.NewGuid();
            rekommendation.CreatedOn = DateTimeOffset.Now;
            rekommendation.UpdatedOn = DateTimeOffset.Now;
            //rekommendation.RekommenderId = rekommenderId;
            rekommendation.Status = RekommendationStatus.EmailToBeVerified;
            rekommendation.Grade = -1;

            _context.Rekommendations.Add(rekommendation);
        }

        //public void RecomputeXpAndRekoAvgFromRekommender(Guid rekommenderId)
        //{
        //    //var rekommender = _context.Rekommenders.FirstOrDefault(a => a.Id == rekommenderId);
        //    if (rekommender != null)
        //    {
        //        IEnumerable<Rekommendation> rekommendationsList = _context.Rekommendations.Where(a => a.RekommenderId == rekommenderId)
        //            .Where(a => a.Status != RekommendationStatus.EmailToBeVerified);
        //        int totalRekoGrade = 0;
        //        int rekommenderXp = 0;
        //        int rekommendationsCount = 0;
        //        foreach(Rekommendation rekommendation in rekommendationsList)
        //        {
        //            if (rekommendation.Grade != -1)
        //            {
        //                totalRekoGrade += rekommendation.Grade;
        //                rekommendationsCount++;
        //            }
        //                switch (rekommendation.Status)
        //            {
        //                case RekommendationStatus.NotViewed:
        //                case RekommendationStatus.Viewed:
        //                    rekommenderXp += 2;
        //                    break;
        //                case RekommendationStatus.Selected:
        //                    rekommenderXp += 8;
        //                    break;
        //                case RekommendationStatus.Accepted:
        //                    rekommenderXp += 20;
        //                    break;
        //                case RekommendationStatus.Rejected:
        //                case RekommendationStatus.Undefined:
        //                default:
        //                    rekommenderXp += 0;
        //                    break;
        //            }
        //        }
                //rekommender.XpRekommend = rekommenderXp;
                //var avgRekoGrade = (double)totalRekoGrade / rekommendationsCount;
                //rekommender.RekommendationsAvgGrade = (int) Math.Round(avgRekoGrade, MidpointRounding.AwayFromZero);
            //}
        //}

        public void UpdateTechJobOpening(TechJobOpening techJobOpening)
        {
            // No code necessary in this implementation
        }

        public void UpdateExtendedUser(ExtendedUser recruiter)
        {
            // No code necessary in this implementation
        }

        public void UpdateCompany(Company company)
        {
            // No code necessary in this implementation
        }

        public void UpdateRekommendation(Rekommendation rekommendation)
        {
            // No code necessary in this implementation
        }

        public void DeleteTechJobOpening(TechJobOpening techJobOpening)
        {
            _context.TechJobOpenings.Remove(techJobOpening);
        }

        public void DeleteExtendedUser(ExtendedUser recruiter)
        {
            if(recruiter == null)
            {
                throw new ArgumentNullException(nameof(recruiter));
            }

            _context.Recruiters.Remove(recruiter);
        }

        public void DeleteCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            _context.Companies.Remove(company);
        }

        public void DeleteRekommendation(Rekommendation rekommendation)
        {
            if (rekommendation == null)
            {
                throw new ArgumentNullException(nameof(rekommendation));
            }

            _context.Rekommendations.Remove(rekommendation);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        public bool IsAuthorizedToPublish(Guid recruiterId)
        {
            if(recruiterId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(recruiterId));
            }

            return _context.Recruiters.Any(a => a.UserId == recruiterId);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(_context != null)
                {
                    _context.Dispose();
                }
            }
        }
    }
}
