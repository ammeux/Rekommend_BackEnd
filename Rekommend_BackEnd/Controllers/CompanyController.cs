using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Extensions;
using Rekommend_BackEnd.Models;
using Rekommend_BackEnd.Repositories;
using Rekommend_BackEnd.ResourceParameters;
using Rekommend_BackEnd.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Net.Http.Headers;
using static Rekommend_BackEnd.Utils.RekomEnums;
using Marvin.Cache.Headers;

namespace Rekommend_BackEnd.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompanyController : ControllerBase
    {
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly IRekommendRepository _repository;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(IRekommendRepository repository, IPropertyCheckerService propertyCheckerService, IPropertyMappingService propertyMappingService, ILogger<CompanyController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 120)]
        [HttpCacheValidation(MustRevalidate = true)]
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet("{companyId}", Name = "GetCompany")]
        [HttpHead("{companyId}", Name = "GetCompany")]
        public IActionResult GetCompany(Guid companyId, string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                _logger.LogInformation($"Media type header value [{mediaType}] not parsable");
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<CompanyDto>(fields))
            {
                return BadRequest();
            }

            var companyFromRepo = _repository.GetCompany(companyId);

            if (companyFromRepo == null)
            {
                _logger.LogInformation($"Company with id [{companyId}] wasn't found when GetCompany");
                return NotFound();
            }

            var companyDto = new CompanyDto
            {
                Id = companyFromRepo.Id,
                RegistrationDate = companyFromRepo.RegistrationDate,
                Name = companyFromRepo.Name,
                HqCity = companyFromRepo.HqCity,
                HqCountry = companyFromRepo.HqCountry,
                PostCode = companyFromRepo.PostCode,
                CompanyDescription = companyFromRepo.CompanyDescription,
                Category = companyFromRepo.Category.ToString(),
                LogoFileName = companyFromRepo.LogoFileName,
                Website = companyFromRepo.Website,
                EmployerBrandWebsite = companyFromRepo.EmployerBrandWebsite
            };

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForCompany(companyId, fields);
            }

            var companyToReturn = companyDto.ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                companyToReturn.Add("links", links);
            }

            return Ok(companyToReturn);
        }

        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 60)]
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet(Name = "GetCompanies")]
        [HttpHead(Name = "GetCompanies")]
        public IActionResult GetCompanies([FromQuery] CompaniesResourceParameters companiesResourceParameters, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                _logger.LogInformation($"Media type header value [{mediaType}] not parsable");
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<CompanyDto>(companiesResourceParameters.Fields))
            {
                _logger.LogInformation($"Property checker did not find on of the Company resource parameters fields");
                return BadRequest();
            }

            if (!_propertyMappingService.ValidMappingExistsFor<CompanyDto, Company>
                (companiesResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var companiesFromRepo = _repository.GetCompanies(companiesResourceParameters);

            var paginationMetadata = new
            {
                totalCount = companiesFromRepo.TotalCount,
                pageSize = companiesFromRepo.PageSize,
                currentPage = companiesFromRepo.CurrentPage,
                totalPages = companiesFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForCompanies(companiesResourceParameters, companiesFromRepo.HasNext, companiesFromRepo.HasPrevious);

            IEnumerable<CompanyDto> companies;

            IList<CompanyDto> companiesList = new List<CompanyDto>();

            foreach (var company in companiesFromRepo)
            {
                companiesList.Add(new CompanyDto()
                {
                    Id = company.Id,
                    RegistrationDate = company.RegistrationDate,
                    Name = company.Name,
                    HqCity = company.HqCity,
                    HqCountry = company.HqCountry,
                    PostCode = company.PostCode,
                    CompanyDescription = company.CompanyDescription,
                    Category = company.Category.ToString(),
                    LogoFileName = company.LogoFileName,
                    Website = company.Website,
                    EmployerBrandWebsite = company.EmployerBrandWebsite
                });
            }

            companies = companiesList;

            var shapedCompanies = companies.ShapeData(companiesResourceParameters.Fields);

            if (parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                var shapedCompaniesWithLinks = shapedCompanies.Select(companies =>
                {
                    var companiesAsDictionary = companies as IDictionary<string, object>;
                    var companyLinks = CreateLinksForCompany((Guid)companiesAsDictionary["Id"], null);
                    companiesAsDictionary.Add("links", companyLinks);
                    return companiesAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedCompaniesWithLinks,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                return Ok(shapedCompanies);
            }
        }

        [HttpPost(Name = "CreateCompany")]
        public ActionResult<CompanyDto> CreateCompany(CompanyForCreationDto companyForCreationDto)
        {

            var company = new Company
            {
                Name = companyForCreationDto.Name,
                HqCity = companyForCreationDto.HqCity,
                HqCountry = companyForCreationDto.HqCountry,
                PostCode = companyForCreationDto.PostCode,
                CompanyDescription = companyForCreationDto.CompanyDescription,
                Category = companyForCreationDto.Category.ToCompanyCategory(),
                LogoFileName = companyForCreationDto.LogoFileName,
                Website = companyForCreationDto.Website,
                EmployerBrandWebsite = companyForCreationDto.EmployerBrandWebsite
            };

            _repository.AddCompany(company);

            var companyToReturn = new CompanyDto
            {
                Id = company.Id,
                RegistrationDate = company.RegistrationDate,
                Name = company.Name,
                HqCity = company.HqCity,
                HqCountry = company.HqCountry,
                PostCode = company.PostCode,
                CompanyDescription = company.CompanyDescription,
                Category = company.Category.ToString(),
                LogoFileName = company.LogoFileName,
                Website = company.Website,
                EmployerBrandWebsite = company.EmployerBrandWebsite
            };

            var links = CreateLinksForCompany(companyToReturn.Id, null);

            var linkedResourcesToReturn = companyToReturn.ShapeData(null) as IDictionary<string, object>;
            linkedResourcesToReturn.Add("links", links);

            if (_repository.Save())
            {
                return CreatedAtRoute("GetCompany", new { companyId = linkedResourcesToReturn["Id"] }, linkedResourcesToReturn);
            }
            else
            {
                _logger.LogInformation($"Create company cannot be saved on repository");
                return BadRequest();
            }
        }

        [HttpPut("{companyId}")]
        public IActionResult UpdateCompany(Guid companyId, CompanyForUpdateDto companyUpdate)
        {
            var companyFromRepo = _repository.GetCompany(companyId);

            if (companyFromRepo == null)
            {
                return NotFound();
            }

            companyFromRepo.Name = companyUpdate.Name;
            companyFromRepo.HqCity = companyUpdate.HqCity;
            companyFromRepo.HqCountry = companyUpdate.HqCountry;
            companyFromRepo.PostCode = companyUpdate.PostCode;
            companyFromRepo.CompanyDescription = companyUpdate.CompanyDescription;
            companyFromRepo.Category = companyUpdate.Category.ToCompanyCategory();
            companyFromRepo.LogoFileName = companyUpdate.LogoFileName;
            companyFromRepo.Website = companyUpdate.Website;
            companyFromRepo.EmployerBrandWebsite = companyUpdate.EmployerBrandWebsite;

            // Action without any effect
            _repository.UpdateCompany(companyFromRepo);

            _repository.Save();

            return NoContent();
        }

        [HttpPatch("{companyId}")]
        public ActionResult PartiallyUpdateCompany(Guid companyId, JsonPatchDocument<CompanyForUpdateDto> patchDocument)
        {
            var companyFromRepo = _repository.GetCompany(companyId);

            if (companyFromRepo == null)
            {
                return NotFound();
            }

            var companyToPatch = new CompanyForUpdateDto
            {
                Name = companyFromRepo.Name,
                HqCity = companyFromRepo.HqCity,
                HqCountry = companyFromRepo.HqCountry,
                PostCode = companyFromRepo.PostCode,
                CompanyDescription = companyFromRepo.CompanyDescription,
                Category = companyFromRepo.Category.ToString(),
                LogoFileName = companyFromRepo.LogoFileName,
                Website = companyFromRepo.Website,
                EmployerBrandWebsite = companyFromRepo.EmployerBrandWebsite
            };

            patchDocument.ApplyTo(companyToPatch, ModelState);

            if (!TryValidateModel(companyToPatch))
            {
                return ValidationProblem(ModelState);
            }

            companyFromRepo.Name = companyToPatch.Name;
            companyFromRepo.HqCity = companyToPatch.HqCity;
            companyFromRepo.HqCountry = companyToPatch.HqCountry;
            companyFromRepo.PostCode = companyToPatch.PostCode;
            companyFromRepo.CompanyDescription = companyToPatch.CompanyDescription;
            companyFromRepo.Category = companyToPatch.Category.ToCompanyCategory();
            companyFromRepo.LogoFileName = companyToPatch.LogoFileName;
            companyFromRepo.Website = companyToPatch.Website;
            companyFromRepo.EmployerBrandWebsite = companyToPatch.EmployerBrandWebsite;

            // Action without any effect
            _repository.UpdateCompany(companyFromRepo);

            _repository.Save();

            return NoContent();
        }

        [HttpDelete("{companyId}", Name = "DeleteCompany")]
        public ActionResult DeleteCompany(Guid companyId)
        {
            var companyFromRepo = _repository.GetCompany(companyId);

            if (companyFromRepo == null)
            {
                return NotFound();
            }

            _repository.DeleteCompany(companyFromRepo);

            _repository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, HEAD, OPTIONS, POST, PUT, PATCH, DELETE");
            return Ok();
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        private IEnumerable<LinkDto> CreateLinksForCompanies(CompaniesResourceParameters companiesResourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(new LinkDto(CreateCompaniesResourceUri(companiesResourceParameters, ResourceUriType.Current), "self", "GET"));
            if (hasNext)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(companiesResourceParameters, ResourceUriType.NextPage), "nextPage", "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(companiesResourceParameters, ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(Guid companyId, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetCompanies", new { companyId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link("GetCompanies", new { companyId, fields }),
                    "self",
                    "GET"));
            }
            links.Add(
                    new LinkDto(Url.Link("DeleteCompany", new { companyId }),
                    "delete_company",
                    "DELETE"));
            return links;
        }

        private string CreateCompaniesResourceUri(CompaniesResourceParameters companiesResourceParameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCompanies",
                        new
                        {
                            fields = companiesResourceParameters.Fields,
                            pageNumber = companiesResourceParameters.PageNumber - 1,
                            pageSize = companiesResourceParameters.PageSize,
                            name = companiesResourceParameters.Name,
                            hqCity = companiesResourceParameters.HqCity,
                            hqCountry = companiesResourceParameters.HqCountry,
                            category = companiesResourceParameters.Category,
                            orderBy = companiesResourceParameters.OrderBy
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCompanies",
                        new
                        {
                            fields = companiesResourceParameters.Fields,
                            pageNumber = companiesResourceParameters.PageNumber + 1,
                            pageSize = companiesResourceParameters.PageSize,
                            name = companiesResourceParameters.Name,
                            hqCity = companiesResourceParameters.HqCity,
                            hqCountry = companiesResourceParameters.HqCountry,
                            category = companiesResourceParameters.Category,
                            orderBy = companiesResourceParameters.OrderBy
                        });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetCompanies",
                        new
                        {
                            fields = companiesResourceParameters.Fields,
                            pageNumber = companiesResourceParameters.PageNumber,
                            pageSize = companiesResourceParameters.PageSize,
                            name = companiesResourceParameters.Name,
                            hqCity = companiesResourceParameters.HqCity,
                            hqCountry = companiesResourceParameters.HqCountry,
                            category = companiesResourceParameters.Category,
                            orderBy = companiesResourceParameters.OrderBy
                        });
            }
        }
    }
}
