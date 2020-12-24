using Microsoft.EntityFrameworkCore;
using Rekommend_BackEnd.DbContexts;
using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Extensions;
using Rekommend_BackEnd.Models;
using Rekommend_BackEnd.ResourceParameters;
using Rekommend_BackEnd.Services;
using Rekommend_BackEnd.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Repositories
{
    public class RekommendRepository : IRekommendRepository
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

        public PagedList<TechJobOpening> GetTechJobOpenings(TechJobOpeningsResourceParameters techJobOpeningsResourceParameters)
        {
            if(techJobOpeningsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(techJobOpeningsResourceParameters));
            }

            var collection = _context.TechJobOpenings.Include(d => d.Recruiter).Include(d => d.Recruiter.Company) as IQueryable<TechJobOpening>;

            var companyCategory = techJobOpeningsResourceParameters.CompanyCategory.ToCompanyCategory();
            if (companyCategory != CompanyCategory.Undefined)
            {
                collection = collection.Where(a => a.Recruiter.Company.Category == companyCategory);
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
                collection = collection.Where(a => a.Recruiter.Company.Name.ToLower().Contains(searchQuery.ToLower()));
            }

            if(!string.IsNullOrWhiteSpace(techJobOpeningsResourceParameters.OrderBy))
            {
                // get property mapping dictionary
                var techJobOpeningMappingDictionary = _propertyMappingService.GetPropertyMapping<TechJobOpeningDto, TechJobOpening>();

                collection = collection.ApplySort(techJobOpeningsResourceParameters.OrderBy, techJobOpeningMappingDictionary);
            }

            return PagedList<TechJobOpening>.Create(collection, techJobOpeningsResourceParameters.PageNumber, techJobOpeningsResourceParameters.PageSize);
        }

        //public IDictionary<Guid, Recruiter> GetRecruiters(IEnumerable<Guid> recruiterIds)
        //{
        //    if(recruiterIds == null)
        //    {
        //        throw new ArgumentNullException(nameof(recruiterIds));
        //    }

        //    return _context.Recruiters.Where(a => recruiterIds.Contains(a.Id)).ToDictionary(x=>x.Id, y=>y);
        //}

        public PagedList<Recruiter> GetRecruiters(RecruitersResourceParameters recruitersResourceParameters)
        {
            if (recruitersResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(recruitersResourceParameters));
            }

            var collection = _context.Recruiters as IQueryable<Recruiter>;

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
                var recruiterMappingDictionary = _propertyMappingService.GetPropertyMapping<RecruiterDto, Recruiter>();

                collection = collection.ApplySort(recruitersResourceParameters.OrderBy, recruiterMappingDictionary);
            }

            return PagedList<Recruiter>.Create(collection, recruitersResourceParameters.PageNumber, recruitersResourceParameters.PageSize);
        }

        //public IEnumerable<Recruiter> GetCompanies(IEnumerable<Guid> companyIds)
        //{
        //    if (companyIds == null)
        //    {
        //        throw new ArgumentNullException(nameof(companyIds));
        //    }

        //    return _context.Recruiters.Where(a => companyIds.Contains(a.Id));
        //}

        public PagedList<Company> GetCompanies(CompaniesResourceParameters companiesResourceParameters)
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

            return PagedList<Company>.Create(collection, companiesResourceParameters.PageNumber, companiesResourceParameters.PageSize);
        }

        public Recruiter GetRecruiter(Guid recruiterId)
        {
            return _context.Recruiters.FirstOrDefault(a => a.Id == recruiterId);
        }

        public void AddRecruiter(Guid companyId, Recruiter recruiter)
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
            recruiter.RegistrationDate = DateTimeOffset.Now;
            recruiter.CompanyId = companyId;
            _context.Recruiters.Add(recruiter);
        }

        public Company GetCompany(Guid companyId)
        {
            return _context.Companies.FirstOrDefault(a => a.Id == companyId);
        }

        public void AddCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            company.Id = Guid.NewGuid();
            company.RegistrationDate = DateTimeOffset.Now;
            _context.Companies.Add(company);
        }

        public TechJobOpening GetTechJobOpening(Guid techJobOpeningId)
        {
            if (techJobOpeningId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(techJobOpeningId));
            }

            return _context.TechJobOpenings.Include(d => d.Recruiter).Include(d => d.Recruiter.Company).FirstOrDefault(a => a.Id == techJobOpeningId);
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
            techJobOpening.CreationDate = DateTimeOffset.Now;
            techJobOpening.RecruiterId = recruiterId;
            _context.TechJobOpenings.Add(techJobOpening);
        }

        public Rekommendation GetRekommendation(Guid rekommendationId)
        {
            return _context.Rekommendations.Include(d => d.Rekommender).Include(d => d.TechJobOpening).FirstOrDefault(a => a.Id == rekommendationId);
        }

        public PagedList<Rekommendation> GetRekommendations(RekommendationsResourceParameters rekommendationsResourceParameters)
        {
            if (rekommendationsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(rekommendationsResourceParameters));
            }

            var collection = _context.Rekommendations.Include(d=>d.Rekommender).Include(d=>d.TechJobOpening) as IQueryable<Rekommendation>;

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

            return PagedList<Rekommendation>.Create(collection, rekommendationsResourceParameters.PageNumber, rekommendationsResourceParameters.PageSize);
        }

        public void AddRekommendation(Guid rekommenderId, Rekommendation rekommendation)
        {
            if (rekommendation == null)
            {
                throw new ArgumentNullException(nameof(rekommendation));
            }

            rekommendation.Id = Guid.NewGuid();
            rekommendation.CreationDate = DateTimeOffset.Now;
            rekommendation.StatusChangeDate = DateTimeOffset.Now;
            rekommendation.RekommenderId = rekommenderId;
            rekommendation.Status = RekommendationStatus.EmailToBeVerified;
            rekommendation.Grade = -1;

            _context.Rekommendations.Add(rekommendation);
        }

        public void RecomputeXpAndRekoAvgFromRekommender(Guid rekommenderId)
        {
            var rekommender = _context.Rekommenders.FirstOrDefault(a => a.Id == rekommenderId);
            if (rekommender != null)
            {
                IEnumerable<Rekommendation> rekommendationsList = _context.Rekommendations.Where(a => a.RekommenderId == rekommenderId)
                    .Where(a => a.Status != RekommendationStatus.EmailToBeVerified);
                int totalRekoGrade = 0;
                int rekommenderXp = 0;
                int rekommendationsCount = 0;
                foreach(Rekommendation rekommendation in rekommendationsList)
                {
                    if (rekommendation.Grade != -1)
                    {
                        totalRekoGrade += rekommendation.Grade;
                        rekommendationsCount++;
                    }
                        switch (rekommendation.Status)
                    {
                        case RekommendationStatus.NotViewed:
                        case RekommendationStatus.Viewed:
                            rekommenderXp += 2;
                            break;
                        case RekommendationStatus.Selected:
                            rekommenderXp += 8;
                            break;
                        case RekommendationStatus.Accepted:
                            rekommenderXp += 20;
                            break;
                        case RekommendationStatus.Rejected:
                        case RekommendationStatus.Undefined:
                        default:
                            rekommenderXp += 0;
                            break;
                    }
                }
                rekommender.XpRekommend = rekommenderXp;
                var avgRekoGrade = (double)totalRekoGrade / rekommendationsCount;
                rekommender.RekommendationsAvgGrade = (int) Math.Round(avgRekoGrade, MidpointRounding.AwayFromZero);
            }
        }

        public Rekommender GetRekommender(Guid rekommenderId)
        {
            return _context.Rekommenders.FirstOrDefault(a => a.Id == rekommenderId);
        }

        public PagedList<Rekommender> GetRekommenders(RekommendersResourceParameters rekommendersResourceParameters)
        {
            if (rekommendersResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(rekommendersResourceParameters));
            }

            var collection = _context.Rekommenders as IQueryable<Rekommender>;

            var position = rekommendersResourceParameters.Position;
            if (position != null)
            {
                collection = collection.Where(a => a.Position.ToString() == position);
            }

            var seniority = rekommendersResourceParameters.Seniority;
            if (seniority != null)
            {
                collection = collection.Where(a => a.Seniority.ToString() == seniority);
            }

            var company = rekommendersResourceParameters.Company;
            if (company != null)
            {
                collection = collection.Where(a => a.Company == company);
            }

            var xpRekommend = rekommendersResourceParameters.XpRekommend;
            if (xpRekommend > 0)
            {
                collection = collection.Where(a => a.XpRekommend > xpRekommend);
            }

            var rekommendationsAvgGrade = rekommendersResourceParameters.RekommendationsAvgGrade;
            if (rekommendationsAvgGrade > 0)
            {
                collection = collection.Where(a => a.RekommendationsAvgGrade > rekommendationsAvgGrade);
            }

            if (!string.IsNullOrWhiteSpace(rekommendersResourceParameters.SearchQuery))
            {
                var searchQuery = rekommendersResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.LastName.ToLower().Contains(searchQuery.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(rekommendersResourceParameters.OrderBy))
            {
                // get property mapping dictionary
                var rekommenderMappingDictionary = _propertyMappingService.GetPropertyMapping<RekommenderDto, Rekommender>();

                collection = collection.ApplySort(rekommendersResourceParameters.OrderBy, rekommenderMappingDictionary);
            }

            return PagedList<Rekommender>.Create(collection, rekommendersResourceParameters.PageNumber, rekommendersResourceParameters.PageSize);
        }

        public void AddRekommender(Rekommender rekommender)
        {
            if (rekommender == null)
            {
                throw new ArgumentNullException(nameof(Rekommender));
            }

            rekommender.Id = Guid.NewGuid();
            rekommender.RegistrationDate = DateTimeOffset.Now;
            rekommender.RekommendationsAvgGrade = 0;
            rekommender.XpRekommend = 0;

            _context.Rekommenders.Add(rekommender);
        }

        public void UpdateTechJobOpening(TechJobOpening techJobOpening)
        {
            // No code necessary in this implementation
        }

        public void UpdateRecruiter(Recruiter recruiter)
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

        public void UpdateRekommender(Rekommender rekommender)
        {
            // No code necessary in this implementation
        }

        public void DeleteTechJobOpening(TechJobOpening techJobOpening)
        {
            _context.TechJobOpenings.Remove(techJobOpening);
        }

        public void DeleteRecruiter(Recruiter recruiter)
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

        public void DeleteRekommender(Rekommender rekommender)
        {
            if (rekommender == null)
            {
                throw new ArgumentNullException(nameof(rekommender));
            }

            _context.Rekommenders.Remove(rekommender);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool IsAuthorizedToPublish(Guid recruiterId)
        {
            if(recruiterId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(recruiterId));
            }

            return _context.Recruiters.Any(a => a.Id == recruiterId);
        }
    }
}
