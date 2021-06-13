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
using static Rekommend_BackEnd.Utils.RekomEnums;
using Marvin.Cache.Headers;
using System.Threading.Tasks;
using Rekommend_BackEnd.Filters;
using Microsoft.Net.Http.Headers;

namespace Rekommend_BackEnd.Controllers
{
    [ApiController]
    [Route("api/rekommendations")]
    public class RekommendationController : ControllerBase
    {
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly IRekommendRepository _repository;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ILogger<CompanyController> _logger;
        private readonly IUserInfoService _userInfoService;

        public RekommendationController(IRekommendRepository repository, IPropertyCheckerService propertyCheckerService, IPropertyMappingService propertyMappingService, ILogger<CompanyController> logger, IUserInfoService userInfoService)
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
        [HttpGet("{rekommendationId}", Name = "GetRekommendation")]
        [HttpHead("{rekommendationId}", Name = "GetRekommendation")]
        [RekommendationFilter]
        public async Task<IActionResult> GetRekommendation(Guid rekommendationId, string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                _logger.LogInformation($"Media type header value [{mediaType}] not parsable");
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<RekommendationDto>(fields))
            {
                return BadRequest();
            }

            var rekommendationFromRepo = await _repository.GetRekommendationAsync(rekommendationId);

            if (rekommendationFromRepo == null)
            {
                _logger.LogInformation($"Rekommendation with id [{rekommendationId}] wasn't found when GetRekommendation");
                return NotFound();
            }
            else
            {
                return Ok(rekommendationFromRepo);
            }
        }

        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 60)]
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet(Name = "GetRekommendations")]
        [HttpHead(Name = "GetRekommendations")]
        [RekommendationsFilter]
        public async Task<IActionResult> GetRekommendations([FromQuery] RekommendationsResourceParameters rekommendationResourceParameters, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                _logger.LogInformation($"Media type header value [{mediaType}] not parsable");
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<RekommendationDto>(rekommendationResourceParameters.Fields))
            {
                _logger.LogInformation($"Property checker did not find on of the Rekommendation resource parameters fields");
                return BadRequest();
            }

            if (!_propertyMappingService.ValidMappingExistsFor<RekommendationDto, Rekommendation>
                (rekommendationResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var rekommendationsFromRepo = await _repository.GetRekommendationsAsync(rekommendationResourceParameters);

            return Ok(rekommendationsFromRepo);
        }

        [HttpPost(Name = "CreateRekommendation")]
        [RekommendationFilter]
        public async Task<ActionResult<RekommendationDto>> CreateRekommendation(RekommendationForCreationDto rekommendationForCreationDto)
        {
            AppUser appUser = new AppUser
            {
                Id = _userInfoService.UserId,
                FirstName = _userInfoService.FirstName,
                LastName = _userInfoService.LastName,
                Email = _userInfoService.Email,
                Company = _userInfoService.Company,
                City = _userInfoService.City,
                Country = _userInfoService.Country,
                Seniority = _userInfoService.Seniority,
                Stack = _userInfoService.Stack,
                Profile = _userInfoService.Profile
            };

            var techJobOpeningId = rekommendationForCreationDto.TechJobOpeningId;

            if (await CheckTechJobOpeningIdIsValid(techJobOpeningId))
            {
                var rekommendation = rekommendationForCreationDto.ToEntity();

                _repository.AddRekommendation(appUser, rekommendation);

                await _repository.SaveChangesAsync();

                // Refetch the Rekommendation from the data store to include the rekommender
                await _repository.GetRekommendationAsync(rekommendation.Id);

                return CreatedAtRoute("GetRekommendation", new { rekommendationId = rekommendation.Id }, rekommendation);
            }
            else
            {
                _logger.LogInformation($"Cannot create Rekommendation as wrong techJobOpeningId [{techJobOpeningId}]");
                return BadRequest();
            }
        }

        private async Task<bool> CheckTechJobOpeningIdIsValid(Guid techJobOpeningId)
        {
            var techJobOpening = await _repository.GetTechJobOpeningAsync(techJobOpeningId);
            if(techJobOpening != null)
            {
                return techJobOpening.Status == JobOfferStatus.Open;
            }
            else
            {
                return false;
            }
        }

        [HttpPut("{rekommendationId}")]
        public async Task<IActionResult> UpdateRekommendation(Guid rekommendationId, RekommendationForUpdateDto rekommendationUpdate)
        {
            var rekommendationFromRepo = await _repository.GetRekommendationAsync(rekommendationId);

            if (rekommendationFromRepo == null)
            {
                return NotFound();
            }

            //var oldGrade = rekommendationFromRepo.Grade;
            //var oldStatus = rekommendationFromRepo.Status;

            // Need to keep repoInstance for Entity Framework
            ApplyUpdateToEntity(rekommendationFromRepo, rekommendationUpdate);

            //bool isRekommenderToBeUpdated = false;
            //if ((rekommendationFromRepo.Grade != oldGrade && rekommendationFromRepo.Grade != -1) || (rekommendationFromRepo.Status != oldStatus && rekommendationFromRepo.Status != RekommendationStatus.EmailToBeVerified))
            //{
            //    isRekommenderToBeUpdated = true;
            //}
            
            // Action without any effect
            _repository.UpdateRekommendation(rekommendationFromRepo);

            //if (isRekommenderToBeUpdated)
            //{
            //    _repository.RecomputeXpAndRekoAvgFromRekommender(rekommendationFromRepo.RekommenderId);
            //}

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{rekommendationId}")]
        public async Task<ActionResult> PartiallyUpdateRekommendation(Guid rekommendationId, JsonPatchDocument<RekommendationForUpdateDto> patchDocument)
        {
            var rekommendationFromRepo = await _repository.GetRekommendationAsync(rekommendationId);

            if (rekommendationFromRepo == null)
            {
                return NotFound();
            }

            var rekommendationToPatch = rekommendationFromRepo.ToUpdateDto();

            patchDocument.ApplyTo(rekommendationToPatch, ModelState);

            if (!TryValidateModel(rekommendationToPatch))
            {
                return ValidationProblem(ModelState);
            }

            //var oldGrade = rekommendationFromRepo.Grade;
            //var oldStatus = rekommendationFromRepo.Status;

            // Need to keep repoInstance for Entity Framework
            ApplyUpdateToEntity(rekommendationFromRepo, rekommendationToPatch);

            //bool isRekommenderToBeUpdated = false;

            //if ((rekommendationFromRepo.Grade != oldGrade && rekommendationFromRepo.Grade != -1) || (rekommendationFromRepo.Status != oldStatus && rekommendationFromRepo.Status != RekommendationStatus.EmailToBeVerified))
            //{
            //    isRekommenderToBeUpdated = true;
            //}
            
            // Action without any effect
            _repository.UpdateRekommendation(rekommendationFromRepo);

            //if (isRekommenderToBeUpdated)
            //{
            //    _repository.RecomputeXpAndRekoAvgFromRekommender(rekommendationFromRepo.RekommenderId);
            //}

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{rekommendationId}", Name = "DeleteRekommendation")]
        public async Task<ActionResult> DeleteRekommendation(Guid rekommendationId)
        {
            var rekommendationFromRepo = await _repository.GetRekommendationAsync(rekommendationId);

            if (rekommendationFromRepo == null)
            {
                return NotFound();
            }

            _repository.DeleteRekommendation(rekommendationFromRepo);

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetRekommendationsOptions()
        {
            Response.Headers.Add("Allow", "GET, HEAD, OPTIONS, POST, PUT, PATCH, DELETE");
            return Ok();
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        private void ApplyUpdateToEntity(Rekommendation rekommendation, RekommendationForUpdateDto rekommendationUpdate)
        {
            rekommendation.FirstName = rekommendationUpdate.FirstName;
            rekommendation.LastName = rekommendationUpdate.LastName;
            rekommendation.Position = rekommendationUpdate.Position.ToPosition();
            rekommendation.Seniority = rekommendationUpdate.Seniority.ToSeniority();
            rekommendation.Comment = rekommendationUpdate.Comment;
            rekommendation.Company = rekommendationUpdate.Company;
            rekommendation.Email = rekommendationUpdate.Email;
            rekommendation.Grade = rekommendationUpdate.Grade;
            rekommendation.Status = rekommendationUpdate.RekommendationStatus.ToRekommendationStatus();
        }
    }
}
