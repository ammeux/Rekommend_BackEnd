using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
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
using Microsoft.AspNetCore.Mvc.Infrastructure;
using static Rekommend_BackEnd.Utils.RekomEnums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Marvin.Cache.Headers;

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

        public TechJobOpeningsController(IRekommendRepository repository, IPropertyCheckerService propertyCheckerService, IPropertyMappingService propertyMappingService, ILogger<TechJobOpeningsController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 120)]
        [HttpCacheValidation(MustRevalidate = true)]
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet("{techJobOpeningId}", Name = "GetTechJobOpening")]
        [HttpHead("{techJobOpeningId}", Name = "GetTechJobOpening")]
        public IActionResult GetTechJobOpening(Guid techJobOpeningId, string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                _logger.LogInformation($"Media type header value [{mediaType}] not parsable");
                return BadRequest();
            }

            if(!_propertyCheckerService.TypeHasProperties<TechJobOpeningDto>(fields))
            {
                return BadRequest();
            }

            var techJobOpeningFromRepo = _repository.GetTechJobOpening(techJobOpeningId);

            if (techJobOpeningFromRepo == null)
            {
                _logger.LogInformation($"TechJobOpening with id [{techJobOpeningId}] wasn't found when GetTechJobOpening");
                return NotFound();
            }

            var techJobOpeningDto = new TechJobOpeningDto
            {
                Id = techJobOpeningFromRepo.Id,
                CreationDate = techJobOpeningFromRepo.CreationDate,
                Title = techJobOpeningFromRepo.Title,
                CompanyId = techJobOpeningFromRepo.Recruiter.CompanyId,
                CompanyName = techJobOpeningFromRepo.Recruiter.Company.Name,
                CompanyCategory = techJobOpeningFromRepo.Recruiter.Company.Category.ToString(),
                RecruiterId = techJobOpeningFromRepo.RecruiterId,
                RecruiterFirstName = techJobOpeningFromRepo.Recruiter.FirstName,
                RecruiterLastName = techJobOpeningFromRepo.Recruiter.LastName,
                RecruiterPosition = techJobOpeningFromRepo.Recruiter.Position.ToString(),
                JobTechLanguage = techJobOpeningFromRepo.JobTechLanguage.ToString(),
                JobPosition = techJobOpeningFromRepo.JobPosition.ToString(),
                Seniority = techJobOpeningFromRepo.Seniority.ToString(),
                ContractType = techJobOpeningFromRepo.ContractType.ToString(),
                RemoteWorkAccepted = techJobOpeningFromRepo.RemoteWorkAccepted,
                MissionDescription = techJobOpeningFromRepo.MissionDescription,
                City = techJobOpeningFromRepo.City.ToString(),
                PostCode = techJobOpeningFromRepo.PostCode,
                Country = techJobOpeningFromRepo.Country.ToString(),
                Reward1 = techJobOpeningFromRepo.Reward1,
                Reward2 = techJobOpeningFromRepo.Reward2,
                Reward3 = techJobOpeningFromRepo.Reward3,
                LikesNb = techJobOpeningFromRepo.LikesNb,
                RekommendationsNb = techJobOpeningFromRepo.RekommendationsNb,
                ViewsNb = techJobOpeningFromRepo.ViewsNb,
                MinimumSalary = techJobOpeningFromRepo.MinimumSalary,
                MaximumSalary = techJobOpeningFromRepo.MaximumSalary,
                Status = techJobOpeningFromRepo.Status,
                PictureFileName = techJobOpeningFromRepo.PictureFileName,
                RseDescription = techJobOpeningFromRepo.RseDescription
            };

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForTechJobOpening(techJobOpeningId, fields);
            }

            var techJobOpeningToReturn = techJobOpeningDto.ShapeData(fields) as IDictionary<string, object>;

            if(includeLinks)
            {
                techJobOpeningToReturn.Add("links", links);
            }

            return Ok(techJobOpeningToReturn);
        }

        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 60)]
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet(Name = "GetTechJobOpenings")]
        [HttpHead(Name = "GetTechJobOpenings")]
        public IActionResult GetTechJobOpenings([FromQuery] TechJobOpeningsResourceParameters techJobOpeningsResourceParameters, [FromHeader(Name = "Accept")] string mediaType)
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

            var techJobOpeningsFromRepo = _repository.GetTechJobOpenings(techJobOpeningsResourceParameters);

            var paginationMetadata = new
            {
                totalCount = techJobOpeningsFromRepo.TotalCount,
                pageSize = techJobOpeningsFromRepo.PageSize,
                currentPage = techJobOpeningsFromRepo.CurrentPage,
                totalPages = techJobOpeningsFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForTechJobOpenings(techJobOpeningsResourceParameters, techJobOpeningsFromRepo.HasNext, techJobOpeningsFromRepo.HasPrevious);

            IEnumerable<TechJobOpeningDto> techJobOpenings;

            IList<TechJobOpeningDto> techJobOpeningsList = new List<TechJobOpeningDto>();

            foreach (var techJobOpening in techJobOpeningsFromRepo)
            {
                techJobOpeningsList.Add(new TechJobOpeningDto()
                {
                    Id = techJobOpening.Id,
                    CreationDate = techJobOpening.CreationDate,
                    ClosingDate = techJobOpening.ClosingDate,
                    StartingDate = techJobOpening.StartingDate,
                    Title = techJobOpening.Title,
                    CompanyId = techJobOpening.Recruiter.CompanyId,
                    CompanyName = techJobOpening.Recruiter.Company.Name,
                    CompanyCategory = techJobOpening.Recruiter.Company.Category.ToString(),
                    RecruiterId = techJobOpening.RecruiterId,
                    RecruiterFirstName = techJobOpening.Recruiter.FirstName,
                    RecruiterLastName = techJobOpening.Recruiter.LastName,
                    RecruiterPosition = techJobOpening.Recruiter.Position.ToString(),
                    JobTechLanguage = techJobOpening.JobTechLanguage.ToString(),
                    JobPosition = techJobOpening.JobPosition.ToString(),
                    Seniority = techJobOpening.Seniority.ToString(),
                    ContractType = techJobOpening.ContractType.ToString(),
                    RemoteWorkAccepted = techJobOpening.RemoteWorkAccepted,
                    MissionDescription = techJobOpening.MissionDescription,
                    City = techJobOpening.City.ToString(),
                    PostCode = techJobOpening.PostCode,
                    Country = techJobOpening.Country.ToString(),
                    Reward1 = techJobOpening.Reward1,
                    Reward2 = techJobOpening.Reward2,
                    Reward3 = techJobOpening.Reward3,
                    LikesNb = techJobOpening.LikesNb,
                    RekommendationsNb = techJobOpening.RekommendationsNb,
                    ViewsNb = techJobOpening.ViewsNb,
                    MinimumSalary = techJobOpening.MinimumSalary,
                    MaximumSalary = techJobOpening.MaximumSalary,
                    Status = techJobOpening.Status,
                    PictureFileName = techJobOpening.PictureFileName,
                    RseDescription = techJobOpening.RseDescription
                });
            }

            techJobOpenings = techJobOpeningsList;

            var shapedTechJobOpenings = techJobOpenings.ShapeData(techJobOpeningsResourceParameters.Fields);

            if (parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                var shapedTechJobOpeningsWithLinks = shapedTechJobOpenings.Select(techJobOpenings =>
                {
                    var techJobOpeningAsDictionary = techJobOpenings as IDictionary<string, object>;
                    var techJobOpeningLinks = CreateLinksForTechJobOpening((Guid)techJobOpeningAsDictionary["Id"], null);
                    techJobOpeningAsDictionary.Add("links", techJobOpeningLinks);
                    return techJobOpeningAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedTechJobOpeningsWithLinks,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                return Ok(shapedTechJobOpenings);
            }
        }

        [HttpPost(Name = "CreateTechJobOpening")]
        public ActionResult<TechJobOpeningDto> CreateTechJobOpening(TechJobOpeningForCreationDto techJobOpeningForCreationDto)
        {
            // A modifier lors de l'implementation de l'authentification
            Guid recruiterId = Guid.Parse("40acecde-ba0f-4936-9f70-a4ef44d65ed9");
            if (!_repository.IsAuthorizedToPublish(recruiterId))
            {
                _logger.LogInformation($"Recruiter [{recruiterId}] is not authorised to publish");
                NotFound();
            }

            var techJobOpening = new TechJobOpening
            {
                StartingDate = techJobOpeningForCreationDto.StartingDate,
                Title = techJobOpeningForCreationDto.Title,
                JobTechLanguage = techJobOpeningForCreationDto.JobTechLanguage.ToJobTechLanguage(),
                JobPosition = techJobOpeningForCreationDto.JobPosition.ToPosition(),
                Seniority = techJobOpeningForCreationDto.Seniority.ToSeniority(),
                ContractType = techJobOpeningForCreationDto.ContractType.ToContractType(),
                RemoteWorkAccepted = techJobOpeningForCreationDto.RemoteWorkAccepted,
                MissionDescription = techJobOpeningForCreationDto.MissionDescription,
                City = techJobOpeningForCreationDto.City,
                PostCode = techJobOpeningForCreationDto.PostCode,
                Country = techJobOpeningForCreationDto.Country.ToCountry(),
                MinimumSalary = techJobOpeningForCreationDto.MinimumSalary,
                MaximumSalary = techJobOpeningForCreationDto.MaximumSalary
            };

            if (!string.IsNullOrWhiteSpace(techJobOpeningForCreationDto.Reward1))
            {
                techJobOpening.Reward1 = techJobOpeningForCreationDto.Reward1;
            }

            if (!string.IsNullOrWhiteSpace(techJobOpeningForCreationDto.Reward2))
            {
                techJobOpening.Reward2 = techJobOpeningForCreationDto.Reward2;
            }

            if (!string.IsNullOrWhiteSpace(techJobOpeningForCreationDto.Reward3))
            {
                techJobOpening.Reward3 = techJobOpeningForCreationDto.Reward3;
            }

            if (!string.IsNullOrWhiteSpace(techJobOpeningForCreationDto.PictureFileName))
            {
                techJobOpening.PictureFileName = techJobOpeningForCreationDto.PictureFileName;
            }

            if (!string.IsNullOrWhiteSpace(techJobOpeningForCreationDto.RseDescription))
            {
                techJobOpening.RseDescription = techJobOpeningForCreationDto.RseDescription;
            }

            _repository.AddTechJobOpening(recruiterId, techJobOpening);

            Recruiter recruiter = _repository.GetRecruiter(recruiterId);

            var techJobOpeningToReturn = new TechJobOpeningDto
            {
                Id = techJobOpening.Id,
                CreationDate = techJobOpening.CreationDate,
                Title = techJobOpening.Title,
                CompanyId = recruiter.CompanyId,
                //CompanyName = recruiter.Company.Name,
                //CompanyCategory = recruiter.Company.Category.ToString(),
                RecruiterId = techJobOpening.RecruiterId,
                RecruiterFirstName = recruiter.FirstName,
                RecruiterLastName = recruiter.LastName,
                RecruiterPosition = recruiter.Position.ToString(),
                JobTechLanguage = techJobOpening.JobTechLanguage.ToString(),
                JobPosition = techJobOpening.JobPosition.ToString(),
                Seniority = techJobOpening.Seniority.ToString(),
                ContractType = techJobOpening.ContractType.ToString(),
                RemoteWorkAccepted = techJobOpening.RemoteWorkAccepted,
                MissionDescription = techJobOpening.MissionDescription,
                City = techJobOpening.City.ToString(),
                PostCode = techJobOpening.PostCode,
                Country = techJobOpening.Country.ToString(),
                Reward1 = techJobOpening.Reward1,
                Reward2 = techJobOpening.Reward2,
                Reward3 = techJobOpening.Reward3,
                LikesNb = techJobOpening.LikesNb,
                RekommendationsNb = techJobOpening.RekommendationsNb,
                ViewsNb = techJobOpening.ViewsNb,
                MinimumSalary = techJobOpening.MinimumSalary,
                MaximumSalary = techJobOpening.MaximumSalary,
                Status = techJobOpening.Status,
                PictureFileName = techJobOpening.PictureFileName,
                RseDescription = techJobOpening.RseDescription
            };

            var links = CreateLinksForTechJobOpening(techJobOpeningToReturn.Id, null);

            var linkedResourcesToReturn = techJobOpeningToReturn.ShapeData(null) as IDictionary<string, object>;
            linkedResourcesToReturn.Add("links", links);

            if (_repository.Save())
            {
                return CreatedAtRoute("GetTechJobOpening", new { techJobOpeningId = linkedResourcesToReturn["Id"] }, linkedResourcesToReturn);
            }
            else
            {
                _logger.LogInformation($"Create techJobOpening cannot be saved on repository");
                return BadRequest();
            }
        }

        [HttpPut("{techJobOpeningId}")]
        public IActionResult UpdateTechJobOpening(Guid techJobOpeningId, TechJobOpeningForUpdateDto techJobOpeningUpdate)
        {
            var techJobOpeningFromRepo = _repository.GetTechJobOpening(techJobOpeningId);

            if(techJobOpeningFromRepo == null)
            {
                return NotFound();
            }

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
            techJobOpeningFromRepo.Reward1 = techJobOpeningUpdate.Reward1;
            techJobOpeningFromRepo.Reward2 = techJobOpeningUpdate.Reward2;
            techJobOpeningFromRepo.Reward3 = techJobOpeningUpdate.Reward3;
            techJobOpeningFromRepo.PictureFileName = techJobOpeningUpdate.PictureFileName;
            techJobOpeningFromRepo.RseDescription = techJobOpeningUpdate.RseDescription;

            // Action without any effect
            _repository.UpdateTechJobOpening(techJobOpeningFromRepo);

            _repository.Save();

            return NoContent();
        }

        [HttpPatch("{techJobOpeningId}")]
        public ActionResult PartiallyUpdateTechJobOpening(Guid techJobOpeningId, JsonPatchDocument<TechJobOpeningForUpdateDto> patchDocument)
        {
            var techJobOpeningFromRepo = _repository.GetTechJobOpening(techJobOpeningId);

            if (techJobOpeningFromRepo == null)
            {
                return NotFound();
            }

            var techJobOpeningToPatch = new TechJobOpeningForUpdateDto
            {
                StartingDate = techJobOpeningFromRepo.StartingDate,
                Title = techJobOpeningFromRepo.Title,
                JobTechLanguage = techJobOpeningFromRepo.JobTechLanguage.ToString(),
                JobPosition = techJobOpeningFromRepo.JobPosition.ToString(),
                Seniority = techJobOpeningFromRepo.Seniority.ToString(),
                ContractType = techJobOpeningFromRepo.ContractType.ToString(),
                RemoteWorkAccepted = techJobOpeningFromRepo.RemoteWorkAccepted,
                MissionDescription = techJobOpeningFromRepo.MissionDescription,
                City = techJobOpeningFromRepo.City.ToString(),
                PostCode = techJobOpeningFromRepo.PostCode,
                Country = techJobOpeningFromRepo.Country.ToString(),
                MinimumSalary = techJobOpeningFromRepo.MinimumSalary,
                MaximumSalary = techJobOpeningFromRepo.MaximumSalary,
                Reward1 = techJobOpeningFromRepo.Reward1,
                Reward2 = techJobOpeningFromRepo.Reward2,
                Reward3 = techJobOpeningFromRepo.Reward3,
                PictureFileName = techJobOpeningFromRepo.PictureFileName,
                RseDescription = techJobOpeningFromRepo.RseDescription
            };

            patchDocument.ApplyTo(techJobOpeningToPatch, ModelState);

            if(!TryValidateModel(techJobOpeningToPatch))
            {
                return ValidationProblem(ModelState);
            }

            techJobOpeningFromRepo.StartingDate = techJobOpeningToPatch.StartingDate;
            techJobOpeningFromRepo.Title = techJobOpeningToPatch.Title;
            techJobOpeningFromRepo.JobTechLanguage = techJobOpeningToPatch.JobTechLanguage.ToJobTechLanguage();
            techJobOpeningFromRepo.JobPosition = techJobOpeningToPatch.JobPosition.ToPosition();
            techJobOpeningFromRepo.Seniority = techJobOpeningToPatch.Seniority.ToSeniority();
            techJobOpeningFromRepo.ContractType = techJobOpeningToPatch.ContractType.ToContractType();
            techJobOpeningFromRepo.RemoteWorkAccepted = techJobOpeningToPatch.RemoteWorkAccepted;
            techJobOpeningFromRepo.MissionDescription = techJobOpeningToPatch.MissionDescription;
            techJobOpeningFromRepo.City = techJobOpeningToPatch.City;
            techJobOpeningFromRepo.PostCode = techJobOpeningToPatch.PostCode;
            techJobOpeningFromRepo.Country = techJobOpeningToPatch.Country.ToCountry();
            techJobOpeningFromRepo.MinimumSalary = techJobOpeningToPatch.MinimumSalary;
            techJobOpeningFromRepo.MaximumSalary = techJobOpeningToPatch.MaximumSalary;
            techJobOpeningFromRepo.Reward1 = techJobOpeningToPatch.Reward1;
            techJobOpeningFromRepo.Reward2 = techJobOpeningToPatch.Reward2;
            techJobOpeningFromRepo.Reward3 = techJobOpeningToPatch.Reward3;
            techJobOpeningFromRepo.PictureFileName = techJobOpeningToPatch.PictureFileName;
            techJobOpeningFromRepo.RseDescription = techJobOpeningToPatch.RseDescription;

            // Action without any effect
            _repository.UpdateTechJobOpening(techJobOpeningFromRepo);

            _repository.Save();

            return NoContent();
        }

        [HttpDelete("{techJobOpeningId}", Name = "DeleteTechJobOpening")]
        public ActionResult DeleteTechJobOpening(Guid techJobOpeningId)
        {
            var techJobOpeningFromRepo = _repository.GetTechJobOpening(techJobOpeningId);

            if (techJobOpeningFromRepo == null)
            {
                return NotFound();
            }

            _repository.DeleteTechJobOpening(techJobOpeningFromRepo);
            _repository.Save();

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

        private IEnumerable<LinkDto> CreateLinksForTechJobOpenings(TechJobOpeningsResourceParameters techJobOpeningsResourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(new LinkDto(CreateTechJobOpeningsResourceUri(techJobOpeningsResourceParameters, ResourceUriType.Current), "self", "GET"));
            if(hasNext)
            {
                links.Add(new LinkDto(CreateTechJobOpeningsResourceUri(techJobOpeningsResourceParameters, ResourceUriType.NextPage), "nextPage", "GET"));
            }
            if(hasPrevious)
            {
                links.Add(new LinkDto(CreateTechJobOpeningsResourceUri(techJobOpeningsResourceParameters, ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForTechJobOpening(Guid techJobOpeningId, string fields)
        {
            var links = new List<LinkDto>();

            if(string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetTechJobOpenings", new { techJobOpeningId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link("GetTechJobOpenings", new { techJobOpeningId, fields }),
                    "self",
                    "GET"));
            }
            links.Add(
                    new LinkDto(Url.Link("DeleteTechJobOpening", new { techJobOpeningId }),
                    "delete_techJobOpening",
                    "DELETE"));
            return links;
        }

        private string CreateTechJobOpeningsResourceUri(TechJobOpeningsResourceParameters techJobOpeningsResourceParameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetTechJobOpenings",
                        new
                        {
                            fields = techJobOpeningsResourceParameters.Fields,
                            orderBy = techJobOpeningsResourceParameters.OrderBy,
                            pageNumber = techJobOpeningsResourceParameters.PageNumber - 1,
                            pageSize = techJobOpeningsResourceParameters.PageSize,
                            companyCategory = techJobOpeningsResourceParameters.CompanyCategory,
                            city = techJobOpeningsResourceParameters.City,
                            remoteWorkAccepted = techJobOpeningsResourceParameters.RemoteWorkAccepted,
                            contractType = techJobOpeningsResourceParameters.ContractType
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetTechJobOpenings",
                        new
                        {
                            fields = techJobOpeningsResourceParameters.Fields,
                            orderBy = techJobOpeningsResourceParameters.OrderBy,
                            pageNumber = techJobOpeningsResourceParameters.PageNumber + 1,
                            pageSize = techJobOpeningsResourceParameters.PageSize,
                            companyCategory = techJobOpeningsResourceParameters.CompanyCategory,
                            city = techJobOpeningsResourceParameters.City,
                            remoteWorkAccepted = techJobOpeningsResourceParameters.RemoteWorkAccepted,
                            contractType = techJobOpeningsResourceParameters.ContractType
                        });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetTechJobOpenings",
                        new
                        {
                            fields = techJobOpeningsResourceParameters.Fields,
                            orderBy = techJobOpeningsResourceParameters.OrderBy,
                            pageNumber = techJobOpeningsResourceParameters.PageNumber,
                            pageSize = techJobOpeningsResourceParameters.PageSize,
                            companyCategory = techJobOpeningsResourceParameters.CompanyCategory,
                            city = techJobOpeningsResourceParameters.City,
                            remoteWorkAccepted = techJobOpeningsResourceParameters.RemoteWorkAccepted,
                            contractType = techJobOpeningsResourceParameters.ContractType
                        });
            }
        }
    }
}
