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
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using static Rekommend_BackEnd.Utils.RekomEnums;
using Marvin.Cache.Headers;

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
        public IActionResult GetRekommender(Guid rekommenderId, string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                _logger.LogInformation($"Media type header value [{mediaType}] not parsable");
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<RekommenderDto>(fields))
            {
                return BadRequest();
            }

            var rekommenderFromRepo = _repository.GetRekommender(rekommenderId);

            if (rekommenderFromRepo == null)
            {
                _logger.LogInformation($"Rekommender with id [{rekommenderId}] wasn't found when GetRekommender");
                return NotFound();
            }

            var rekommenderDto = new RekommenderDto
            {
                Id = rekommenderFromRepo.Id,
                Age = rekommenderFromRepo.DateOfBirth.GetCurrentAge(),
                RegistrationDate = rekommenderFromRepo.RegistrationDate,
                FirstName = rekommenderFromRepo.FirstName,
                LastName = rekommenderFromRepo.LastName,
                Position = rekommenderFromRepo.Position.ToString(),
                Seniority = rekommenderFromRepo.Seniority.ToString(),
                Company = rekommenderFromRepo.Company,
                City = rekommenderFromRepo.City,
                PostCode = rekommenderFromRepo.PostCode,
                Email = rekommenderFromRepo.Email,
                XpRekommend = rekommenderFromRepo.XpRekommend,
                RekommendationsAvgGrade = rekommenderFromRepo.RekommendationsAvgGrade,
                Level = GetRekommenderLevel(rekommenderFromRepo.XpRekommend)
            };

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForRekommender(rekommenderId, fields);
            }

            var rekommenderToReturn = rekommenderDto.ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                rekommenderToReturn.Add("links", links);
            }

            return Ok(rekommenderToReturn);
        }

        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 60)]
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet(Name = "GetRekommenders")]
        [HttpHead(Name = "GetRekommenders")]
        public IActionResult GetRekommenders([FromQuery] RekommendersResourceParameters rekommenderResourceParameters, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                _logger.LogInformation($"Media type header value [{mediaType}] not parsable");
                return BadRequest();
            }

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

            var rekommendersFromRepo = _repository.GetRekommenders(rekommenderResourceParameters);

            var paginationMetadata = new
            {
                totalCount = rekommendersFromRepo.TotalCount,
                pageSize = rekommendersFromRepo.PageSize,
                currentPage = rekommendersFromRepo.CurrentPage,
                totalPages = rekommendersFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForRekommenders(rekommenderResourceParameters, rekommendersFromRepo.HasNext, rekommendersFromRepo.HasPrevious);

            IEnumerable<RekommenderDto> rekommenders;

            IList<RekommenderDto> rekommendersList = new List<RekommenderDto>();

            foreach (var rekommender in rekommendersFromRepo)
            {
                rekommendersList.Add(new RekommenderDto()
                {
                    Id = rekommender.Id,
                    Age = rekommender.DateOfBirth.GetCurrentAge(),
                    RegistrationDate = rekommender.RegistrationDate,
                    FirstName = rekommender.FirstName,
                    LastName = rekommender.LastName,
                    Position = rekommender.Position.ToString(),
                    Seniority = rekommender.Seniority.ToString(),
                    Company = rekommender.Company,
                    City = rekommender.City,
                    Email = rekommender.Email,
                    PostCode = rekommender.PostCode,
                    XpRekommend = rekommender.XpRekommend,
                    RekommendationsAvgGrade = rekommender.RekommendationsAvgGrade,
                    Level = GetRekommenderLevel(rekommender.XpRekommend)
                });
            }

            rekommenders = rekommendersList;

            var shapedRekommenders = rekommenders.ShapeData(rekommenderResourceParameters.Fields);

            if (parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                var shapedRekommendersWithLinks = shapedRekommenders.Select(rekommenders =>
                {
                    var rekommendersAsDictionary = rekommenders as IDictionary<string, object>;
                    var rekommenderLinks = CreateLinksForRekommender((Guid)rekommendersAsDictionary["Id"], null);
                    rekommendersAsDictionary.Add("links", rekommenderLinks);
                    return rekommendersAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedRekommendersWithLinks,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                return Ok(shapedRekommenders);
            }
        }

        [HttpPost(Name = "CreateRekommender")]
        public ActionResult<Rekommender> CreateRekommender(RekommenderForCreationDto rekommenderForCreationDto)
        {
            var rekommender = new Rekommender
            {
                DateOfBirth = rekommenderForCreationDto.DateOfBirth,
                FirstName = rekommenderForCreationDto.FirstName,
                LastName = rekommenderForCreationDto.LastName,
                Position = rekommenderForCreationDto.Position.ToPosition(),
                Seniority = rekommenderForCreationDto.Seniority.ToSeniority(),
                Company = rekommenderForCreationDto.Company,
                City = rekommenderForCreationDto.City,
                PostCode = rekommenderForCreationDto.PostCode,
                Email = rekommenderForCreationDto.Email
            };

            _repository.AddRekommender(rekommender);

            var rekommenderToReturn = new RekommenderDto
            {
                Id = rekommender.Id,
                Age = rekommender.DateOfBirth.GetCurrentAge(),
                RegistrationDate = rekommender.RegistrationDate,
                FirstName = rekommender.FirstName,
                LastName = rekommender.LastName,
                Position = rekommender.Position.ToString(),
                Seniority = rekommender.Seniority.ToString(),
                Company = rekommender.Company,
                City = rekommender.City,
                PostCode = rekommender.PostCode,
                Email = rekommender.Email,
                XpRekommend = rekommender.XpRekommend,
                RekommendationsAvgGrade = rekommender.RekommendationsAvgGrade,
                Level = GetRekommenderLevel(rekommender.XpRekommend)
            };

            var links = CreateLinksForRekommender(rekommenderToReturn.Id, null);

            var linkedResourcesToReturn = rekommenderToReturn.ShapeData(null) as IDictionary<string, object>;
            linkedResourcesToReturn.Add("links", links);

            if (_repository.Save())
            {
                return CreatedAtRoute("GetRekommender", new { rekommenderId = linkedResourcesToReturn["Id"] }, linkedResourcesToReturn);
            }
            else
            {
                _logger.LogInformation($"Create rekommender cannot be saved on repository");
                return BadRequest();
            }
        }

        [HttpPut("{rekommenderId}")]
        public IActionResult UpdateRekommender(Guid rekommenderId, RekommenderForUpdateDto rekommenderUpdate)
        {
            var rekommenderFromRepo = _repository.GetRekommender(rekommenderId);

            if (rekommenderFromRepo == null)
            {
                return NotFound();
            }

            rekommenderFromRepo.FirstName = rekommenderUpdate.FirstName;
            rekommenderFromRepo.LastName = rekommenderUpdate.LastName;
            rekommenderFromRepo.Position = rekommenderUpdate.Position.ToPosition();
            rekommenderFromRepo.Seniority = rekommenderUpdate.Seniority.ToSeniority();
            rekommenderFromRepo.Company = rekommenderUpdate.Company;
            rekommenderFromRepo.City = rekommenderUpdate.City;
            rekommenderFromRepo.PostCode = rekommenderUpdate.PostCode;
            rekommenderFromRepo.Email = rekommenderUpdate.Email;

            // Action without any effect
            _repository.UpdateRekommender(rekommenderFromRepo);

            _repository.Save();

            return NoContent();
        }

        [HttpPatch("{rekommenderId}")]
        public ActionResult PartiallyUpdateRekommender(Guid rekommenderId, JsonPatchDocument<RekommenderForUpdateDto> patchDocument)
        {
            var rekommenderFromRepo = _repository.GetRekommender(rekommenderId);

            if (rekommenderFromRepo == null)
            {
                return NotFound();
            }

            var rekommenderToPatch = new RekommenderForUpdateDto
            {
                FirstName = rekommenderFromRepo.FirstName,
                LastName = rekommenderFromRepo.LastName,
                Position = rekommenderFromRepo.Position.ToString(),
                Seniority = rekommenderFromRepo.Seniority.ToString(),
                City = rekommenderFromRepo.City,
                PostCode = rekommenderFromRepo.PostCode,
                Company = rekommenderFromRepo.Company,
                Email = rekommenderFromRepo.Email
            };

            patchDocument.ApplyTo(rekommenderToPatch, ModelState);

            if (!TryValidateModel(rekommenderToPatch))
            {
                return ValidationProblem(ModelState);
            }

            rekommenderFromRepo.FirstName = rekommenderToPatch.FirstName;
            rekommenderFromRepo.LastName = rekommenderToPatch.LastName;
            rekommenderFromRepo.Position = rekommenderToPatch.Position.ToPosition();
            rekommenderFromRepo.Seniority = rekommenderToPatch.Seniority.ToSeniority();
            rekommenderFromRepo.City = rekommenderToPatch.City;
            rekommenderFromRepo.Email = rekommenderToPatch.Email;
            rekommenderFromRepo.PostCode = rekommenderToPatch.PostCode;

            // Action without any effect
            _repository.UpdateRekommender(rekommenderFromRepo);

            _repository.Save();

            return NoContent();
        }

        private string GetRekommenderLevel(int xpRekommend)
        {
            if (xpRekommend < 50)
            {
                return "Apprentice";
            }
            else if (xpRekommend < 200)
            {
                return "Craftsman";
            }
            else if (xpRekommend < 500)
            {
                return "Master";
            }
            else if (xpRekommend < 1000)
            {
                return "Giant";
            }
            else
            {
                return "Hero";
            }
        }

        [HttpDelete("{rekommenderId}", Name = "DeleteRekommender")]
        public ActionResult DeleteRekommender(Guid rekommenderId)
        {
            var rekommenderFromRepo = _repository.GetRekommender(rekommenderId);

            if (rekommenderFromRepo == null)
            {
                return NotFound();
            }

            _repository.DeleteRekommender(rekommenderFromRepo);

            _repository.Save();

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

        private IEnumerable<LinkDto> CreateLinksForRekommenders(RekommendersResourceParameters rekommendersResourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(new LinkDto(CreateRekommendersResourceUri(rekommendersResourceParameters, ResourceUriType.Current), "self", "GET"));
            if (hasNext)
            {
                links.Add(new LinkDto(CreateRekommendersResourceUri(rekommendersResourceParameters, ResourceUriType.NextPage), "nextPage", "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateRekommendersResourceUri(rekommendersResourceParameters, ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForRekommender(Guid rekommenderId, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetRekommender", new { rekommenderId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link("GetRekommenders", new { rekommenderId, fields }),
                    "self",
                    "GET"));
            }
            links.Add(
                    new LinkDto(Url.Link("DeleteRekommender", new { rekommenderId }),
                    "delete_rekommender",
                    "DELETE"));
            return links;
        }

        private string CreateRekommendersResourceUri(RekommendersResourceParameters rekommendersResourceParameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetRekommendations",
                        new
                        {
                            fields = rekommendersResourceParameters.Fields,
                            pageNumber = rekommendersResourceParameters.PageNumber - 1,
                            pageSize = rekommendersResourceParameters.PageSize,
                            position = rekommendersResourceParameters.Position,
                            seniority = rekommendersResourceParameters.Seniority,
                            xpRekommend = rekommendersResourceParameters.XpRekommend,
                            RekommendationsAvgGrade = rekommendersResourceParameters.RekommendationsAvgGrade,
                            level = rekommendersResourceParameters.Level,
                            orderBy = rekommendersResourceParameters.OrderBy
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCompanies",
                        new
                        {
                            fields = rekommendersResourceParameters.Fields,
                            pageNumber = rekommendersResourceParameters.PageNumber + 1,
                            pageSize = rekommendersResourceParameters.PageSize,
                            position = rekommendersResourceParameters.Position,
                            seniority = rekommendersResourceParameters.Seniority,
                            xpRekommend = rekommendersResourceParameters.XpRekommend,
                            RekommendationsAvgGrade = rekommendersResourceParameters.RekommendationsAvgGrade,
                            level = rekommendersResourceParameters.Level,
                            orderBy = rekommendersResourceParameters.OrderBy
                        });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetCompanies",
                        new
                        {
                            fields = rekommendersResourceParameters.Fields,
                            pageNumber = rekommendersResourceParameters.PageNumber,
                            pageSize = rekommendersResourceParameters.PageSize,
                            position = rekommendersResourceParameters.Position,
                            seniority = rekommendersResourceParameters.Seniority,
                            xpRekommend = rekommendersResourceParameters.XpRekommend,
                            RekommendationsAvgGrade = rekommendersResourceParameters.RekommendationsAvgGrade,
                            level = rekommendersResourceParameters.Level,
                            orderBy = rekommendersResourceParameters.OrderBy
                        });
            }
        }
    }
}
