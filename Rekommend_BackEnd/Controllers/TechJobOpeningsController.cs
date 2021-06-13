using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Extensions;
using Rekommend_BackEnd.Models;
using Rekommend_BackEnd.Repositories;
using Rekommend_BackEnd.ResourceParameters;
using Rekommend_BackEnd.Services;
using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Marvin.Cache.Headers;
using System.Threading.Tasks;
using Rekommend_BackEnd.Filters;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Rekommend_BackEnd.Controllers
{
    [ApiController]
    [Route("api/techJobOpenings")]
    public class TechJobOpeningsController : ControllerBase
    {
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly IRekommendRepository _repository;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ILogger<TechJobOpeningsController> _logger;
        private readonly IUserInfoService _userInfoService;

        public TechJobOpeningsController(IRekommendRepository repository, IPropertyCheckerService propertyCheckerService, IPropertyMappingService propertyMappingService, ILogger<TechJobOpeningsController> logger, IUserInfoService userInfoService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        }

        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 120)]
        [HttpCacheValidation(MustRevalidate = true)]
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet("{techJobOpeningId}", Name = "GetTechJobOpening")]
        [HttpHead("{techJobOpeningId}", Name = "GetTechJobOpening")]
        [TechJobOpeningFilter]
        public async Task<IActionResult> GetTechJobOpening(Guid techJobOpeningId, string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                _logger.LogInformation($"Media type header value [{mediaType}] not parsable");
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<TechJobOpeningDto>(fields))
            {
                return BadRequest();
            }

            var techJobOpeningFromRepo = await _repository.GetTechJobOpeningAsync(techJobOpeningId);

            if (techJobOpeningFromRepo == null)
            {
                _logger.LogInformation($"TechJobOpening with id [{techJobOpeningId}] wasn't found when GetTechJobOpening");
                return NotFound();
            }
            else
            {
                return Ok(techJobOpeningFromRepo);
            }
        }

        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 60)]
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet(Name = "GetTechJobOpenings")]
        [HttpHead(Name = "GetTechJobOpenings")]
        [TechJobOpeningsFilter]
        [Authorize]
        public async Task<IActionResult> GetTechJobOpenings([FromQuery] TechJobOpeningsResourceParameters techJobOpeningsResourceParameters, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                _logger.LogInformation($"Media type header value [{mediaType}] not parsable");
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<TechJobOpeningDto>(techJobOpeningsResourceParameters.Fields))
            {
                _logger.LogInformation($"Property checker did not find on of the TechJobOpening resource parameters fields");
                return BadRequest();
            }

            if (!_propertyMappingService.ValidMappingExistsFor<TechJobOpeningDto, TechJobOpening>
                (techJobOpeningsResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var techJobOpeningsFromRepo = await _repository.GetTechJobOpeningsAsync(techJobOpeningsResourceParameters);

            return Ok(techJobOpeningsFromRepo);
        }

        [HttpPost(Name = "CreateTechJobOpening")]
        [TechJobOpeningFilter]
        [Authorize("MustBeARecruiter")]
        public async Task<ActionResult<TechJobOpeningDto>> CreateTechJobOpening(TechJobOpeningForCreationDto techJobOpeningForCreationDto)
        {
            var techJobOpening = techJobOpeningForCreationDto.ToEntity();

            if (!string.IsNullOrWhiteSpace(techJobOpeningForCreationDto.Reward))
            {
                techJobOpening.Reward = techJobOpeningForCreationDto.Reward;
            }

            if (!string.IsNullOrWhiteSpace(techJobOpeningForCreationDto.BonusReward))
            {
                techJobOpening.BonusReward = techJobOpeningForCreationDto.BonusReward;
            }

            if (!string.IsNullOrWhiteSpace(techJobOpeningForCreationDto.PictureFileName))
            {
                techJobOpening.PictureFileName = techJobOpeningForCreationDto.PictureFileName;
            }

            if (!string.IsNullOrWhiteSpace(techJobOpeningForCreationDto.RseDescription))
            {
                techJobOpening.RseDescription = techJobOpeningForCreationDto.RseDescription;
            }

            techJobOpening.CreatedByFirstName = _userInfoService.FirstName;
            techJobOpening.CreatedByLastName = _userInfoService.LastName;

            techJobOpening.CompanyId = _repository.GetExtendedUserAsync(_userInfoService.UserId).Result.CompanyId;

            _repository.AddTechJobOpening(_userInfoService.UserId, techJobOpening);

            await _repository.SaveChangesAsync();

            // Refetch the techJobOpening from the data store to include the recruiter
            await _repository.GetTechJobOpeningAsync(techJobOpening.Id);
            
            return CreatedAtRoute("GetTechJobOpening", new { techJobOpeningId = techJobOpening.Id }, techJobOpening);
        }

        [HttpPut("{techJobOpeningId}")]
        public async Task<IActionResult> UpdateTechJobOpening(Guid techJobOpeningId, TechJobOpeningForUpdateDto techJobOpeningUpdate)
        {
            var techJobOpeningFromRepo = await _repository.GetTechJobOpeningAsync(techJobOpeningId);

            if(techJobOpeningFromRepo == null)
            {
                return NotFound();
            }

            // Need to keep repoInstance for Entity Framework
            ApplyUpdateToEntity(techJobOpeningFromRepo, techJobOpeningUpdate);

            // Action without any effect
            _repository.UpdateTechJobOpening(techJobOpeningFromRepo);

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{techJobOpeningId}")]
        public async Task<ActionResult> PartiallyUpdateTechJobOpening(Guid techJobOpeningId, JsonPatchDocument<TechJobOpeningForUpdateDto> patchDocument)
        {
            var techJobOpeningFromRepo = await _repository.GetTechJobOpeningAsync(techJobOpeningId);

            if (techJobOpeningFromRepo == null)
            {
                return NotFound();
            }

            var techJobOpeningToPatch = techJobOpeningFromRepo.ToUpdateDto();

            patchDocument.ApplyTo(techJobOpeningToPatch, ModelState);

            if(!TryValidateModel(techJobOpeningToPatch))
            {
                return ValidationProblem(ModelState);
            }

            // Need to keep repoInstance for Entity Framework
            ApplyUpdateToEntity(techJobOpeningFromRepo, techJobOpeningToPatch);

            // Action without any effect
            _repository.UpdateTechJobOpening(techJobOpeningFromRepo);

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{techJobOpeningId}", Name = "DeleteTechJobOpening")]
        public async Task<ActionResult> DeleteTechJobOpening(Guid techJobOpeningId)
        {
            var techJobOpeningFromRepo = await _repository.GetTechJobOpeningAsync(techJobOpeningId);

            if (techJobOpeningFromRepo == null)
            {
                return NotFound();
            }

            _repository.DeleteTechJobOpening(techJobOpeningFromRepo);
            
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetTechJobOpeingsOptions()
        {
            Response.Headers.Add("Allow", "GET, HEAD, OPTIONS, POST, PUT, PATCH, DELETE");
            return Ok();
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        
        private void ApplyUpdateToEntity(TechJobOpening techJobOpeningFromRepo, TechJobOpeningForUpdateDto techJobOpeningUpdate)
        {
            techJobOpeningFromRepo.StartingDate = techJobOpeningUpdate.StartingDate;
            techJobOpeningFromRepo.Title = techJobOpeningUpdate.Title;
            techJobOpeningFromRepo.JobTechLanguage = techJobOpeningUpdate.JobTechLanguage.ToJobTechLanguage();
            techJobOpeningFromRepo.JobPosition = techJobOpeningUpdate.JobPosition.ToPosition();
            techJobOpeningFromRepo.Seniority = techJobOpeningUpdate.Seniority.ToSeniority();
            techJobOpeningFromRepo.ContractType = techJobOpeningUpdate.ContractType.ToContractType();
            techJobOpeningFromRepo.RemoteWorkAccepted = techJobOpeningUpdate.RemoteWorkAccepted;
            techJobOpeningFromRepo.MissionDescription = techJobOpeningUpdate.MissionDescription;
            techJobOpeningFromRepo.City = techJobOpeningUpdate.City;
            techJobOpeningFromRepo.PostCode = techJobOpeningUpdate.PostCode;
            techJobOpeningFromRepo.Country = techJobOpeningUpdate.Country.ToCountry();
            techJobOpeningFromRepo.MinimumSalary = techJobOpeningUpdate.MinimumSalary;
            techJobOpeningFromRepo.MaximumSalary = techJobOpeningUpdate.MaximumSalary;
            techJobOpeningFromRepo.Reward = techJobOpeningUpdate.Reward;
            techJobOpeningFromRepo.BonusReward = techJobOpeningUpdate.BonusReward;
            techJobOpeningFromRepo.PictureFileName = techJobOpeningUpdate.PictureFileName;
            techJobOpeningFromRepo.RseDescription = techJobOpeningUpdate.RseDescription;
        }
    }
}
