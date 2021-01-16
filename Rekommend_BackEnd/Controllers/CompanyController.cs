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
using Marvin.Cache.Headers;
using Rekommend_BackEnd.Filters;
using System.Threading.Tasks;

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
        [CompanyFilter]
        public async Task<IActionResult> GetCompany(Guid companyId, string fields)
        {
            if (!_propertyCheckerService.TypeHasProperties<CompanyDto>(fields))
            {
                return BadRequest();
            }

            var companyFromRepo = await _repository.GetCompanyAsync(companyId);

            if (companyFromRepo == null)
            {
                _logger.LogInformation($"Company with id [{companyId}] wasn't found when GetCompany");
                return NotFound();
            }
            else
            {
                return Ok(companyFromRepo);
            }
        }

        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 60)]
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet(Name = "GetCompanies")]
        [HttpHead(Name = "GetCompanies")]
        [CompaniesFilter]
        public async Task<IActionResult> GetCompanies([FromQuery] CompaniesResourceParameters companiesResourceParameters)
        {
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

            var companiesFromRepo = await _repository.GetCompaniesAsync(companiesResourceParameters);

            return Ok(companiesFromRepo);
        }

        [HttpPost(Name = "CreateCompany")]
        [CompanyFilter]
        public async Task<ActionResult<CompanyDto>> CreateCompany(CompanyForCreationDto companyForCreationDto)
        {

            var company = companyForCreationDto.ToEntity();

            _repository.AddCompany(company);

            await _repository.SaveChangesAsync();
            
            return CreatedAtRoute("GetCompany", new { companyId = company.Id }, company);
        }

        [HttpPut("{companyId}")]
        public async Task<IActionResult> UpdateCompany(Guid companyId, CompanyForUpdateDto companyUpdate)
        {
            var companyFromRepo = await _repository.GetCompanyAsync(companyId);

            if (companyFromRepo == null)
            {
                return NotFound();
            }

            companyFromRepo = companyUpdate.ToEntity();

            // Action without any effect
            _repository.UpdateCompany(companyFromRepo);

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{companyId}")]
        public async Task<ActionResult> PartiallyUpdateCompany(Guid companyId, JsonPatchDocument<CompanyForUpdateDto> patchDocument)
        {
            var companyFromRepo = await _repository.GetCompanyAsync(companyId);

            if (companyFromRepo == null)
            {
                return NotFound();
            }

            var companyToPatch = companyFromRepo.ToUpdateDto();

            patchDocument.ApplyTo(companyToPatch, ModelState);

            if (!TryValidateModel(companyToPatch))
            {
                return ValidationProblem(ModelState);
            }

            companyFromRepo = companyToPatch.ToEntity();

            // Action without any effect
            _repository.UpdateCompany(companyFromRepo);

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{companyId}", Name = "DeleteCompany")]
        public async Task<ActionResult> DeleteCompany(Guid companyId)
        {
            var companyFromRepo = await _repository.GetCompanyAsync(companyId);

            if (companyFromRepo == null)
            {
                return NotFound();
            }

            _repository.DeleteCompany(companyFromRepo);

            await _repository.SaveChangesAsync();

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
    }
}
