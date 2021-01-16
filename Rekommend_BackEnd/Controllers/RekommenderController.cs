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
    [Route("api/rekommenders")]
    public class RekommenderController : ControllerBase
    {
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly IRekommendRepository _repository;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ILogger<CompanyController> _logger;

        public RekommenderController(IRekommendRepository repository, IPropertyCheckerService propertyCheckerService, IPropertyMappingService propertyMappingService, ILogger<CompanyController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 120)]
        [HttpCacheValidation(MustRevalidate = true)]
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet("{rekommenderId}", Name = "GetRekommender")]
        [HttpHead("{rekommenderId}", Name = "GetRekommender")]
        [RekommenderFilter]
        public async Task<IActionResult> GetRekommender(Guid rekommenderId, string fields)
        {
            if (!_propertyCheckerService.TypeHasProperties<RekommenderDto>(fields))
            {
                return BadRequest();
            }

            var rekommenderFromRepo = await _repository.GetRekommenderAsync(rekommenderId);

            if (rekommenderFromRepo == null)
            {
                _logger.LogInformation($"Rekommender with id [{rekommenderId}] wasn't found when GetRekommender");
                return NotFound();
            }
            else
            {
                return Ok(rekommenderFromRepo);
            }
        }

        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 60)]
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet(Name = "GetRekommenders")]
        [HttpHead(Name = "GetRekommenders")]
        [RekommendersFilter]
        public async Task<IActionResult> GetRekommenders([FromQuery] RekommendersResourceParameters rekommenderResourceParameters)
        {
            if (!_propertyCheckerService.TypeHasProperties<RekommenderDto>(rekommenderResourceParameters.Fields))
            {
                _logger.LogInformation($"Property checker did not find on of the Rekommender resource parameters fields");
                return BadRequest();
            }

            if (!_propertyMappingService.ValidMappingExistsFor<RekommenderDto, Rekommender>
                (rekommenderResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var rekommendersFromRepo = await _repository.GetRekommendersAsync(rekommenderResourceParameters);
                
            return Ok(rekommendersFromRepo);
        }

        [HttpPost(Name = "CreateRekommender")]
        [RekommenderFilter]
        public async Task<ActionResult<Rekommender>> CreateRekommender(RekommenderForCreationDto rekommenderForCreationDto)
        {
            var rekommender = rekommenderForCreationDto.ToEntity();

            _repository.AddRekommender(rekommender);

            await _repository.SaveChangesAsync();
            
            return CreatedAtRoute("GetRekommender", new { rekommenderId = rekommender.Id }, rekommender);
        }

        [HttpPut("{rekommenderId}")]
        public async Task<IActionResult> UpdateRekommender(Guid rekommenderId, RekommenderForUpdateDto rekommenderUpdate)
        {
            var rekommenderFromRepo = await _repository.GetRekommenderAsync(rekommenderId);

            if (rekommenderFromRepo == null)
            {
                return NotFound();
            }

            rekommenderFromRepo = rekommenderUpdate.ToEntity();

            // Action without any effect
            _repository.UpdateRekommender(rekommenderFromRepo);

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{rekommenderId}")]
        public async Task<ActionResult> PartiallyUpdateRekommender(Guid rekommenderId, JsonPatchDocument<RekommenderForUpdateDto> patchDocument)
        {
            var rekommenderFromRepo = await _repository.GetRekommenderAsync(rekommenderId);

            if (rekommenderFromRepo == null)
            {
                return NotFound();
            }

            var rekommenderToPatch = rekommenderFromRepo.ToUpdateDto();

            patchDocument.ApplyTo(rekommenderToPatch, ModelState);

            if (!TryValidateModel(rekommenderToPatch))
            {
                return ValidationProblem(ModelState);
            }

            rekommenderFromRepo = rekommenderToPatch.ToEntity();

            // Action without any effect
            _repository.UpdateRekommender(rekommenderFromRepo);

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{rekommenderId}", Name = "DeleteRekommender")]
        public async Task<ActionResult> DeleteRekommender(Guid rekommenderId)
        {
            var rekommenderFromRepo = await _repository.GetRekommenderAsync(rekommenderId);

            if (rekommenderFromRepo == null)
            {
                return NotFound();
            }

            _repository.DeleteRekommender(rekommenderFromRepo);

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetRekommendersOptions()
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
