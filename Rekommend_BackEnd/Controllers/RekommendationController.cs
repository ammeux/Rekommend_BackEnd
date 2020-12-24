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
using System.Net.Http.Headers;
using System.Text.Json;
using static Rekommend_BackEnd.Utils.RekomEnums;

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

        public RekommendationController(IRekommendRepository repository, IPropertyCheckerService propertyCheckerService, IPropertyMappingService propertyMappingService, ILogger<CompanyController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{rekommendationId}", Name = "GetRekommendation")]
        [HttpHead("{rekommendationId}", Name = "GetRekommendation")]
        public IActionResult GetRekommendation(Guid rekommendationId, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                _logger.LogInformation($"Media type header value [{mediaType}] not parsable");
                return BadRequest();
            }

            var rekommendationFromRepo = _repository.GetRekommendation(rekommendationId);

            if (rekommendationFromRepo == null)
            {
                _logger.LogInformation($"Rekommendation with id [{rekommendationId}] wasn't found when GetRekommendation");
                return NotFound();
            }

            var rekommendationDto = new RekommendationDto
            {
                Id = rekommendationFromRepo.Id,
                CreationDate = rekommendationFromRepo.CreationDate,
                StatusChangeDate = rekommendationFromRepo.StatusChangeDate,
                RekommenderId = rekommendationFromRepo.RekommenderId,
                RekommenderFirstName = rekommendationFromRepo.Rekommender.FirstName,
                RekommenderLastName = rekommendationFromRepo.Rekommender.LastName,
                RekommenderPosition = rekommendationFromRepo.Rekommender.Position.ToString(),
                RekommenderSeniority = rekommendationFromRepo.Rekommender.Seniority.ToString(),
                RekommenderCompany = rekommendationFromRepo.Rekommender.Company,
                RekommenderCity = rekommendationFromRepo.Rekommender.City,
                RekommenderPostCode = rekommendationFromRepo.Rekommender.PostCode,
                RekommenderEmail = rekommendationFromRepo.Rekommender.Email,
                TechJobOpeningId = rekommendationFromRepo.TechJobOpeningId,
                TechJobOpeningTitle = rekommendationFromRepo.TechJobOpening.Title,
                FirstName = rekommendationFromRepo.FirstName,
                LastName = rekommendationFromRepo.LastName,
                Position = rekommendationFromRepo.Position.ToString(),
                Seniority = rekommendationFromRepo.Seniority.ToString(),
                Company = rekommendationFromRepo.Company,
                Email = rekommendationFromRepo.Email,
                Comment = rekommendationFromRepo.Comment,
                Status = rekommendationFromRepo.Status.ToString(),
                HasAlreadyWorkedWithRekommender = rekommendationFromRepo.HasAlreadyWorkedWithRekommender
            };

            return Ok(rekommendationDto);
        }

        [HttpGet(Name = "GetRekommendations")]
        [HttpHead(Name = "GetRekommendations")]
        public IActionResult GetRekommendations([FromQuery] RekommendationsResourceParameters rekommendationResourceParameters, [FromHeader(Name = "Accept")] string mediaType)
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

            var rekommendationsFromRepo = _repository.GetRekommendations(rekommendationResourceParameters);

            var paginationMetadata = new
            {
                totalCount = rekommendationsFromRepo.TotalCount,
                pageSize = rekommendationsFromRepo.PageSize,
                currentPage = rekommendationsFromRepo.CurrentPage,
                totalPages = rekommendationsFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForRekommendations(rekommendationResourceParameters, rekommendationsFromRepo.HasNext, rekommendationsFromRepo.HasPrevious);

            IEnumerable<RekommendationDto> rekommendations;

            IList<RekommendationDto> rekommendationsList = new List<RekommendationDto>();

            foreach (var rekommendation in rekommendationsFromRepo)
            {
                rekommendationsList.Add(new RekommendationDto()
                {
                    Id = rekommendation.Id,
                    CreationDate = rekommendation.CreationDate,
                    StatusChangeDate = rekommendation.StatusChangeDate,
                    RekommenderId = rekommendation.RekommenderId,
                    RekommenderFirstName = rekommendation.Rekommender.FirstName,
                    RekommenderLastName = rekommendation.Rekommender.LastName,
                    RekommenderPosition = rekommendation.Rekommender.Position.ToString(),
                    RekommenderSeniority = rekommendation.Rekommender.Seniority.ToString(),
                    RekommenderCompany = rekommendation.Rekommender.Company,
                    RekommenderCity = rekommendation.Rekommender.City,
                    RekommenderPostCode = rekommendation.Rekommender.PostCode,
                    RekommenderEmail = rekommendation.Rekommender.Email,
                    TechJobOpeningId = rekommendation.TechJobOpeningId,
                    TechJobOpeningTitle = rekommendation.TechJobOpening.Title,
                    FirstName = rekommendation.FirstName,
                    LastName = rekommendation.LastName,
                    Position = rekommendation.Position.ToString(),
                    Seniority = rekommendation.Seniority.ToString(),
                    Company = rekommendation.Company,
                    Email = rekommendation.Email,
                    Comment = rekommendation.Comment,
                    Status = rekommendation.Status.ToString(),
                    HasAlreadyWorkedWithRekommender = rekommendation.HasAlreadyWorkedWithRekommender
                });
            }

            rekommendations = rekommendationsList;

            var shapedRekommendations = rekommendations.ShapeData(rekommendationResourceParameters.Fields);

            if (parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                var shapedRekommendationsWithLinks = shapedRekommendations.Select(shapedRekommendations =>
                {
                    var rekommendationsAsDictionary = rekommendations as IDictionary<string, object>;
                    var rekommendationLinks = CreateLinksForRekommendation((Guid)rekommendationsAsDictionary["Id"], null);
                    rekommendationsAsDictionary.Add("links", rekommendationLinks);
                    return rekommendationsAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedRekommendations,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                return Ok(shapedRekommendations);
            }
        }

        [HttpPost(Name = "CreateRekommendation")]
        public ActionResult<RekommendationDto> CreateRekommendation(RekommendationForCreationDto rekommendationForCreationDto)
        {
            // A modifier lors de l'implementation de l'authentification
            Guid rekommenderId = Guid.Parse("aaaef973-d8ce-4c92-95b4-3635bb2d42d1");

            var techJobOpeningId = rekommendationForCreationDto.TechJobOpeningId;

            if (CheckTechJobOpeningIdIsValid(techJobOpeningId))
            {
                var rekommendation = new Rekommendation
                {
                    TechJobOpeningId = rekommendationForCreationDto.TechJobOpeningId,
                    FirstName = rekommendationForCreationDto.FirstName,
                    LastName = rekommendationForCreationDto.LastName,
                    Position = rekommendationForCreationDto.Position.ToPosition(),
                    Seniority = rekommendationForCreationDto.Seniority.ToSeniority(),
                    Company = rekommendationForCreationDto.Company,
                    Email = rekommendationForCreationDto.Email,
                    Comment = rekommendationForCreationDto.Comment,
                    HasAlreadyWorkedWithRekommender = rekommendationForCreationDto.HasAlreadyWorkedWithRekommender
                };

                _repository.AddRekommendation(rekommenderId, rekommendation);

                if (_repository.Save())
                {
                    return CreatedAtRoute("GetRekommendation", new { rekommendationId = rekommendation.Id }, rekommendation);
                }
                else
                {
                    _logger.LogInformation($"Create rekommendation cannot be saved on repository");
                    return BadRequest();
                }
            }
            else
            {
                _logger.LogInformation($"Cannon create Rekommendation as wrong techJobOpeningId [{techJobOpeningId}]");
                return BadRequest();
            }
        }

        private bool CheckTechJobOpeningIdIsValid(Guid techJobOpeningId)
        {
            var techJobOpening = _repository.GetTechJobOpening(techJobOpeningId);
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
        public IActionResult UpdateRekommendation(Guid rekommendationId, RekommendationForUpdateDto rekommendationUpdate)
        {
            var rekommendationFromRepo = _repository.GetRekommendation(rekommendationId);

            if (rekommendationFromRepo == null)
            {
                return NotFound();
            }

            rekommendationFromRepo.FirstName = rekommendationUpdate.FirstName;
            rekommendationFromRepo.LastName = rekommendationUpdate.LastName;
            rekommendationFromRepo.Position = rekommendationUpdate.Position.ToPosition();
            rekommendationFromRepo.Seniority = rekommendationUpdate.Seniority.ToSeniority();
            rekommendationFromRepo.Comment = rekommendationUpdate.Comment;
            
            rekommendationFromRepo.Company = rekommendationUpdate.Company;
            rekommendationFromRepo.Email = rekommendationUpdate.Email;

            var newGrade = rekommendationUpdate.Grade;
            var newStatus = rekommendationUpdate.RekommendationStatus.ToRekommendationStatus();
            bool isRekommenderToBeUpdated = false;
            if ((rekommendationFromRepo.Grade != newGrade && newGrade != -1) || (rekommendationFromRepo.Status != newStatus && newStatus != RekommendationStatus.EmailToBeVerified))
            {
                isRekommenderToBeUpdated = true;
            }
            rekommendationFromRepo.Grade = newGrade;
            rekommendationFromRepo.Status = newStatus;

            // Action without any effect
            _repository.UpdateRekommendation(rekommendationFromRepo);

            _repository.Save();

            if (isRekommenderToBeUpdated)
            {
                _repository.RecomputeXpAndRekoAvgFromRekommender(rekommendationFromRepo.RekommenderId);
                _repository.Save();
            }

            return NoContent();
        }

        [HttpPatch("{rekommendationId}")]
        public ActionResult PartiallyUpdateRekommendation(Guid rekommendationId, JsonPatchDocument<RekommendationForUpdateDto> patchDocument)
        {
            var rekommendationFromRepo = _repository.GetRekommendation(rekommendationId);

            if (rekommendationFromRepo == null)
            {
                return NotFound();
            }

            var rekommendationToPatch = new RekommendationForUpdateDto
            {
                FirstName = rekommendationFromRepo.FirstName,
                LastName = rekommendationFromRepo.LastName,
                Position = rekommendationFromRepo.Position.ToString(),
                Seniority = rekommendationFromRepo.Seniority.ToString(),
                Comment = rekommendationFromRepo.Comment,
                RekommendationStatus = rekommendationFromRepo.Status.ToString(),
                Company = rekommendationFromRepo.Company,
                Email = rekommendationFromRepo.Email,
                Grade = rekommendationFromRepo.Grade
            };

            patchDocument.ApplyTo(rekommendationToPatch, ModelState);

            if (!TryValidateModel(rekommendationToPatch))
            {
                return ValidationProblem(ModelState);
            }

            rekommendationFromRepo.FirstName = rekommendationToPatch.FirstName;
            rekommendationFromRepo.LastName = rekommendationToPatch.LastName;
            rekommendationFromRepo.Position = rekommendationToPatch.Position.ToPosition();
            rekommendationFromRepo.Seniority = rekommendationToPatch.Seniority.ToSeniority();
            rekommendationFromRepo.Comment = rekommendationToPatch.Comment;
            rekommendationFromRepo.Email = rekommendationToPatch.Email;

            var newStatus =  rekommendationToPatch.RekommendationStatus.ToRekommendationStatus();
            var newGrade = rekommendationToPatch.Grade;
            bool isRekommenderToBeUpdated = false;

            if ((rekommendationFromRepo.Grade != newGrade && newGrade != -1) || (rekommendationFromRepo.Status != newStatus && newStatus != RekommendationStatus.EmailToBeVerified))
            {
                isRekommenderToBeUpdated = true;
            }
            rekommendationFromRepo.Grade = newGrade;
            rekommendationFromRepo.Status = newStatus;

            // Action without any effect
            _repository.UpdateRekommendation(rekommendationFromRepo);

            _repository.Save();

            if (isRekommenderToBeUpdated)
            {
                _repository.RecomputeXpAndRekoAvgFromRekommender(rekommendationFromRepo.RekommenderId);
                _repository.Save();
            }

            return NoContent();
        }

        [HttpDelete("{rekommendationId}", Name = "DeleteRekommendation")]
        public ActionResult DeleteRekommendation(Guid rekommendationId)
        {
            var rekommendationFromRepo = _repository.GetRekommendation(rekommendationId);

            if (rekommendationFromRepo == null)
            {
                return NotFound();
            }

            _repository.DeleteRekommendation(rekommendationFromRepo);

            _repository.Save();

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

        private IEnumerable<LinkDto> CreateLinksForRekommendations(RekommendationsResourceParameters rekommendationsResourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(new LinkDto(CreateRekommendationsResourceUri(rekommendationsResourceParameters, ResourceUriType.Current), "self", "GET"));
            if (hasNext)
            {
                links.Add(new LinkDto(CreateRekommendationsResourceUri(rekommendationsResourceParameters, ResourceUriType.NextPage), "nextPage", "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateRekommendationsResourceUri(rekommendationsResourceParameters, ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForRekommendation(Guid rekommendationId, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetRekommendation", new { rekommendationId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link("GetRekommendations", new { rekommendationId, fields }),
                    "self",
                    "GET"));
            }
            links.Add(
                    new LinkDto(Url.Link("DeleteRekommendation", new { rekommendationId }),
                    "delete_rekommendation",
                    "DELETE"));
            links.Add(
                    new LinkDto(Url.Link("CreateRekommendation", new { rekommendationId }),
                    "create_rekommendation",
                    "POST"));
            return links;
        }

        private string CreateRekommendationsResourceUri(RekommendationsResourceParameters rekommendationsResourceParameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetRekommendations",
                        new
                        {
                            fields = rekommendationsResourceParameters.Fields,
                            pageNumber = rekommendationsResourceParameters.PageNumber - 1,
                            pageSize = rekommendationsResourceParameters.PageSize,
                            techJobOpeningId = rekommendationsResourceParameters.TechJobOpeningId,
                            position = rekommendationsResourceParameters.Position,
                            seniority = rekommendationsResourceParameters.Seniority,
                            rekommendationStatus = rekommendationsResourceParameters.Status,
                            orderBy = rekommendationsResourceParameters.OrderBy
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCompanies",
                        new
                        {
                            fields = rekommendationsResourceParameters.Fields,
                            pageNumber = rekommendationsResourceParameters.PageNumber + 1,
                            pageSize = rekommendationsResourceParameters.PageSize,
                            techJobOpeningId = rekommendationsResourceParameters.TechJobOpeningId,
                            position = rekommendationsResourceParameters.Position,
                            seniority = rekommendationsResourceParameters.Seniority,
                            rekommendationStatus = rekommendationsResourceParameters.Status,
                            orderBy = rekommendationsResourceParameters.OrderBy
                        });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetCompanies",
                        new
                        {
                            fields = rekommendationsResourceParameters.Fields,
                            pageNumber = rekommendationsResourceParameters.PageNumber,
                            pageSize = rekommendationsResourceParameters.PageSize,
                            techJobOpeningId = rekommendationsResourceParameters.TechJobOpeningId,
                            position = rekommendationsResourceParameters.Position,
                            seniority = rekommendationsResourceParameters.Seniority,
                            rekommendationStatus = rekommendationsResourceParameters.Status,
                            orderBy = rekommendationsResourceParameters.OrderBy
                        });
            }
        }
    }
}
