﻿using Rekommend_BackEnd.DbContexts;
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

            var collection = _context.TechJobOpenings as IQueryable<TechJobOpening>;

            if(techJobOpeningsResourceParameters.CompanyCategory != CompanyCategory.Undefined)
            {
                collection = collection.Where(a => a.Recruiter.Company.Category == techJobOpeningsResourceParameters.CompanyCategory);
            }
            if (techJobOpeningsResourceParameters.JobTechLanguage != JobTechLanguage.Undefined)
            {
                collection = collection.Where(a => a.JobTechLanguage == techJobOpeningsResourceParameters.JobTechLanguage);
            }
            if (techJobOpeningsResourceParameters.City != City.Undefined)
            {
                collection = collection.Where(a => a.City == techJobOpeningsResourceParameters.City);
            }
            if(techJobOpeningsResourceParameters.RemoteWorkAccepted == true)
            {
                collection = collection.Where(a => a.RemoteWorkAccepted == true);
            }
            if(techJobOpeningsResourceParameters.ContractType != ContractType.Undefined)
            {
                collection = collection.Where(a => a.ContractType == techJobOpeningsResourceParameters.ContractType);
            }
            if(techJobOpeningsResourceParameters.Position != Position.Undefined)
            {
                collection = collection.Where(a => a.JobPosition == techJobOpeningsResourceParameters.Position);
            }
            if(techJobOpeningsResourceParameters.Seniority != Seniority.Undefined)
            {
                collection = collection.Where(a => a.Seniority == techJobOpeningsResourceParameters.Seniority);
            }
            if(!string.IsNullOrWhiteSpace(techJobOpeningsResourceParameters.SearchQuery))
            {
                var searchQuery = techJobOpeningsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Recruiter.Company.Name.ToLower().Contains(searchQuery.ToLower()));
            }
            HashSet<string> propertiesHashSet;
            if(!string.IsNullOrWhiteSpace(techJobOpeningsResourceParameters.OrderBy) && _entityPropertiesService.TryGetPropertiesHash("TechJobOpening", out propertiesHashSet))
            {
                collection = collection.ApplySort(techJobOpeningsResourceParameters.OrderBy, propertiesHashSet);
            }

            return PagedList<TechJobOpening>.Create(collection, techJobOpeningsResourceParameters.PageNumber, techJobOpeningsResourceParameters.PageSize);
        }
    }
}
