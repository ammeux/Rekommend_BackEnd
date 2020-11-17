using Microsoft.EntityFrameworkCore;
using Rekommend_BackEnd.DbContexts;
using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Extensions;
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
        private readonly IEntityPropertiesService _entityPropertiesService;

        public RekommendRepository(ApplicationContext context, IEntityPropertiesService entityPropertiesService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _entityPropertiesService = entityPropertiesService ?? throw new ArgumentNullException(nameof(entityPropertiesService));
        }

        public PagedList<TechJobOpening> GetTechJobOpenings(TechJobOpeningsResourceParameters techJobOpeningsResourceParameters)
        {
            if(techJobOpeningsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(TechJobOpeningsResourceParameters));
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

            var city = techJobOpeningsResourceParameters.City.ToCity();
            if (city != City.Undefined)
            {
                collection = collection.Where(a => a.City == city);
            }

            if(techJobOpeningsResourceParameters.RemoteWorkAccepted == true)
            {
                collection = collection.Where(a => a.RemoteWorkAccepted == true);
            }

            var contractType = techJobOpeningsResourceParameters.ContractType.toContractType();
            if(contractType != ContractType.Undefined)
            {
                collection = collection.Where(a => a.ContractType == contractType);
            }

            var jobPosition = techJobOpeningsResourceParameters.JobPosition.toJobPosition();
            if(jobPosition != Position.Undefined)
            {
                collection = collection.Where(a => a.JobPosition == jobPosition);
            }

            var seniority = techJobOpeningsResourceParameters.Seniority.toSeniority();
            if(seniority != Seniority.Undefined)
            {
                collection = collection.Where(a => a.Seniority == seniority);
            }

            if(!string.IsNullOrWhiteSpace(techJobOpeningsResourceParameters.SearchQuery))
            {
                var searchQuery = techJobOpeningsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Recruiter.Company.Name.ToLower().Contains(searchQuery.ToLower()));
            }

            return PagedList<TechJobOpening>.Create(collection, techJobOpeningsResourceParameters.PageNumber, techJobOpeningsResourceParameters.PageSize);
        }

        public IDictionary<Guid, Recruiter> GetRecruiters(IEnumerable<Guid> recruiterIds)
        {
            if(recruiterIds == null)
            {
                throw new ArgumentNullException(nameof(recruiterIds));
            }

            return _context.Recruiters.Where(a => recruiterIds.Contains(a.Id)).ToDictionary(x=>x.Id, y=>y);
        }

        public IEnumerable<Recruiter> GetCompanies(IEnumerable<Guid> companyIds)
        {
            if (companyIds == null)
            {
                throw new ArgumentNullException(nameof(companyIds));
            }

            return _context.Recruiters.Where(a => companyIds.Contains(a.Id));
        }

        public Recruiter GetRecruiter(Guid recruiterId)
        {
            return _context.Recruiters.FirstOrDefault(a => a.Id == recruiterId);
        }
        public Company GetCompany(Guid companyId)
        {
            return _context.Companies.FirstOrDefault(a => a.Id == companyId);
        }
    }
}
