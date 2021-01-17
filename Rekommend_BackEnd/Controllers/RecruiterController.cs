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
using Microsoft.Net.Http.Headers;

namespace Rekommend_BackEnd.Controllers
{
    [ApiController]
    [Route("api/recruiters")]
    public class RecruiterController : ControllerBase
    {
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly IRekommendRepository _repository;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ILogger<RecruiterController> _logger;

        public RecruiterController(IRekommendRepository repository, IPropertyCheckerService propertyCheckerService, IPropertyMappingService propertyMappingService, ILogger<RecruiterController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 120)]
        [HttpCacheValidation(MustRevalidate = true)]
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet("{recruiterId}", Name = "GetRecruiter")]
        [HttpHead("{recruiterId}", Name = "GetRecruiter")]
        [RecruiterFilter]
        public async Task<IActionResult> GetRecruiter(Guid recruiterId, string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                _logger.LogInformation($"Media type header value [{mediaType}] not parsable");
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<RecruiterDto>(fields))
            {
                return BadRequest();
            }

            var recruiterFromRepo = await _repository.GetRecruiterAsync(recruiterId);

            if(recruiterFromRepo == null)
            {
                _logger.LogInformation($"Recruiter with id [{recruiterId}] wasn't found when GetRecruiter");
                return NotFound();
            }
            else
            {
                return Ok(recruiterFromRepo);
            }
        }

        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 60)]
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet(Name = "GetRecruiters")]
        [HttpHead(Name = "GetRecruiters")]
        [RecruitersFilter]
        public async Task<IActionResult> GetRecruiters([FromQuery] RecruitersResourceParameters recruitersResourceParameters, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                _logger.LogInformation($"Media type header value [{mediaType}] not parsable");
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<RecruiterDto>(recruitersResourceParameters.Fields))
            {
                _logger.LogInformation($"Property checker did not find on of the Recruiter resource parameters fields");
                return BadRequest();
            }

            if (!_propertyMappingService.ValidMappingExistsFor<RecruiterDto, Recruiter>
                (recruitersResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var recruitersFromRepo = await _repository.GetRecruitersAsync(recruitersResourceParameters);

            return Ok(recruitersFromRepo);
        }

        [HttpPost(Name = "CreateRecruiter")]
        [RecruiterFilter]
        public async Task<ActionResult<RecruiterDto>> CreateRecruiter(RecruiterForCreationDto recruiterForCreationDto)
        {
            // A modifier lors de l'implementation de l'authentification
            Guid companyId = Guid.Parse("e0de73e1-3873-496a-ad69-37334f6f58f3");

            var recruiter = recruiterForCreationDto.ToEntity();

            _repository.AddRecruiter(companyId, recruiter);

            await _repository.SaveChangesAsync();

            // Refetch the recruiter from the data store to include the company
            await _repository.GetRecruiterAsync(recruiter.Id);

            return CreatedAtRoute("GetRecruiter", new { recruiterId = recruiter.Id }, recruiter);
        }

        [HttpPut("{recruiterId}")]
        public async Task<IActionResult> UpdateRecruiter(Guid recruiterId, RecruiterForUpdateDto recruiterUpdate)
        {
            var recruiterFromRepo = await _repository.GetRecruiterAsync(recruiterId);

            if (recruiterFromRepo == null)
            {
                return NotFound();
            }

            // Need to keep repoInstance for Entity Framework
            ApplyUpdateToEntity(recruiterFromRepo, recruiterUpdate);

            // Action without any effect
            _repository.UpdateRecruiter(recruiterFromRepo);

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{recruiterId}")]
        public async Task<ActionResult> PartiallyUpdateRecruiter(Guid recruiterId, JsonPatchDocument<RecruiterForUpdateDto> patchDocument)
        {
            var recruiterFromRepo = await _repository.GetRecruiterAsync(recruiterId);

            if(recruiterFromRepo == null)
            {
                return NotFound();
            }

            var recruiterToPatch = recruiterFromRepo.ToUpdateDto();

            patchDocument.ApplyTo(recruiterToPatch, ModelState);

            if(!TryValidateModel(recruiterToPatch))
            {
                return ValidationProblem(ModelState);
            }

            // Need to keep repoInstance for Entity Framework
            ApplyUpdateToEntity(recruiterFromRepo, recruiterToPatch);

            // Action without any effect
            _repository.UpdateRecruiter(recruiterFromRepo);

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{recruiterId}", Name = "DeleteRecruiter")]
        public async Task<ActionResult> DeleteRecruiter(Guid recruiterId)
        {
            var recruiterFromRepo = await _repository.GetRecruiterAsync(recruiterId);

            if(recruiterFromRepo == null)
            {
                return NotFound();
            }

            _repository.DeleteRecruiter(recruiterFromRepo);

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetRecruitersOptions()
        {
            Response.Headers.Add("Allow", "GET, HEAD, OPTIONS, POST, PUT, PATCH, DELETE");
            return Ok();
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        private void ApplyUpdateToEntity(Recruiter recruiter, RecruiterForUpdateDto recruiterUpdate)
        {
            recruiter.FirstName = recruiterUpdate.FirstName;
            recruiter.LastName = recruiterUpdate.LastName;
            recruiter.Position = recruiterUpdate.Position.ToRecruiterPosition();
            recruiter.DateOfBirth = recruiterUpdate.DateOfBirth;
            recruiter.Email = recruiterUpdate.Email;
            recruiter.Gender = recruiterUpdate.Gender.ToGender();
        }
    }
}
