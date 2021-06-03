using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rekommend_BackEnd.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        private readonly Dictionary<string, PropertyMappingValue> _techJobOpeningPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>(){"Id"}) },
                {"CreatedOn", new PropertyMappingValue(new List<string>(){"CreatedOn"}) },
                {"Title", new PropertyMappingValue(new List<string>() {"Title" })},
                {"JobTechLanguage", new PropertyMappingValue(new List<string>(){"JobTechLanguage"}) },
                {"JobPosition", new PropertyMappingValue(new List<string>() {"JobPosition" }) },
                {"Seniority", new PropertyMappingValue(new List<string>(){"Seniority"}) },
                {"ContractType", new PropertyMappingValue(new List<string>(){"ContractType"}) },
                {"MinimumSalary", new PropertyMappingValue(new List<string>(){"MinimumSalary"}) },
                {"MaximumSalary", new PropertyMappingValue(new List<string>(){"MaximumSalary"}) }
            };

        private readonly Dictionary<string, PropertyMappingValue> _extendedUserPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>(){"Id"}) },
                {"CompanyId", new PropertyMappingValue(new List<string>(){"CompanyId"}) },
                {"Position", new PropertyMappingValue(new List<string>(){"Position"}) },
            };

        private readonly Dictionary<string, PropertyMappingValue> _companyPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>(){"Id"}) },
                {"CreatedOn", new PropertyMappingValue(new List<string>() {"CreatedOn" })},
                {"Name", new PropertyMappingValue(new List<string>(){"Name"}) },
                {"HqCity", new PropertyMappingValue(new List<string>() {"HqCity" }) },
                {"CountryCity", new PropertyMappingValue(new List<string>(){"CountryCity"}) },
                {"Category", new PropertyMappingValue(new List<string>(){"Category"}) },
                {"PostCode", new PropertyMappingValue(new List<string>(){"PostCode"}) },
            };

        private readonly Dictionary<string, PropertyMappingValue> _rekommendationPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>(){"Id"}) },
                {"CreatedOn", new PropertyMappingValue(new List<string>() {"CreatedOn" })},
                {"Rekommender", new PropertyMappingValue(new List<string>(){"Rekommender"}) },
                {"TechJobOpening", new PropertyMappingValue(new List<string>() {"TechJobOpening" }) },
                {"FirstName", new PropertyMappingValue(new List<string>(){"FirstName"}) },
                {"LastName", new PropertyMappingValue(new List<string>(){"LastName"}) },
                {"Position", new PropertyMappingValue(new List<string>(){"Position"}) },
                {"Seniority", new PropertyMappingValue(new List<string>(){"Seniority"}) },
                {"Company", new PropertyMappingValue(new List<string>(){"Company"}) },
                {"Email", new PropertyMappingValue(new List<string>(){"Email"}) },
                {"Comment", new PropertyMappingValue(new List<string>(){"Comment"}) },
                {"Status", new PropertyMappingValue(new List<string>(){"Status"}) },
                {"HasAlreadyWorkedWithRekommender", new PropertyMappingValue(new List<string>(){"HasAlreadyWorkedWithRekommender" }) }
            };

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<TechJobOpeningDto, TechJobOpening>(_techJobOpeningPropertyMapping));
            _propertyMappings.Add(new PropertyMapping<ExtendedUserDto, ExtendedUser>(_extendedUserPropertyMapping));
            _propertyMappings.Add(new PropertyMapping<CompanyDto, Company>(_companyPropertyMapping));
            _propertyMappings.Add(new PropertyMapping<RekommendationDto, Rekommendation>(_rekommendationPropertyMapping));
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');

            // run through the fields clauses
            foreach (var field in fieldsAfterSplit)
            {
                // trim
                var trimmedField = field.Trim();

                // remove everything after the first " " - if the fields 
                // are coming from an orderBy string, this part must be 
                // ignored
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            //get matching mapping
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if(matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }

            throw new Exception("Cannot find exact property mapping instance " + $"for <{typeof(TSource)},{typeof(TDestination)}");
        }
    }
}
