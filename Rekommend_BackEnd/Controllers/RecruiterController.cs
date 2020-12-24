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

        [HttpGet("{recruiterId}", Name = "GetRecruiter")]
        [HttpHead("{recruiterId}", Name = "GetRecruiter")]
        public IActionResult GetRecruiter(Guid recruiterId, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                _logger.LogInformation($"Media type header value [{mediaType}] not parsable");
                return BadRequest();
            }

            var recruiterFromRepo = _repository.GetRecruiter(recruiterId);

            if(recruiterFromRepo == null)
            {
                _logger.LogInformation($"Recruiter with id [{recruiterId}] wasn't found when GetRecruiter");
                return NotFound();
            }

            var recruiterDto = new RecruiterDto
            {
                Id = recruiterFromRepo.Id,
                RegistrationDate = recruiterFromRepo.RegistrationDate,
                FirstName = recruiterFromRepo.FirstName,
                LastName = recruiterFromRepo.LastName,
                CompanyId = recruiterFromRepo.CompanyId,
                Position = recruiterFromRepo.Position.ToString(),
                Age = recruiterFromRepo.DateOfBirth.GetCurrentAge(),
                Email = recruiterFromRepo.Email,
                Gender = recruiterFromRepo.Gender.ToString()
            };

            return Ok(recruiterDto);
        }

        [HttpGet(Name = "GetRecruiters")]
        [HttpHead(Name = "GetRecruiters")]
        public IActionResult GetRecruiters([FromQuery] RecruitersResourceParameters recruitersResourceParameters, [FromHeader(Name = "Accept")] string mediaType)
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

            var recruitersFromRepo = _repository.GetRecruiters(recruitersResourceParameters);

            var paginationMetadata = new
            {
                totalCount = recruitersFromRepo.TotalCount,
                pageSize = recruitersFromRepo.PageSize,
                currentPage = recruitersFromRepo.CurrentPage,
                totalPages = recruitersFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForRecruiters(recruitersResourceParameters, recruitersFromRepo.HasNext, recruitersFromRepo.HasPrevious);

            IEnumerable<RecruiterDto> recruiters;

            IList<RecruiterDto> recruitersList = new List<RecruiterDto>();

            foreach (var recruiter in recruitersFromRepo)
            {
                recruitersList.Add(new RecruiterDto()
                {
                    Id = recruiter.Id,
                    RegistrationDate = recruiter.RegistrationDate,
                    FirstName = recruiter.FirstName,
                    LastName = recruiter.LastName,
                    CompanyId = recruiter.CompanyId,
                    Position = recruiter.Position.ToString(),
                    Age = recruiter.DateOfBirth.GetCurrentAge(),
                    Email = recruiter.Email,
                    Gender = recruiter.Gender.ToString()
                });
            }
            
            recruiters = recruitersList;

            var shapedRecruiters = recruiters.ShapeData(recruitersResourceParameters.Fields);

            if (parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                var shapedRecruitersWithLinks = shapedRecruiters.Select(shapedRecruiters =>
                {
                    var recruiterAsDictionary = recruiters as IDictionary<string, object>;
                    var recruiterLinks = CreateLinksForRecruiter((Guid)recruiterAsDictionary["Id"], null);
                    recruiterAsDictionary.Add("links", recruiterLinks);
                    return recruiterAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedRecruiters,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                return Ok(shapedRecruiters);
            }
        }

        [HttpPost(Name = "CreateRecruiter")]
        public ActionResult<RecruiterDto> CreateRecruiter(RecruiterForCreationDto recruiterForCreationDto)
        {
            // A modifier lors de l'implementation de l'authentification
            Guid companyId = Guid.Parse("e0de73e1-3873-496a-ad69-37334f6f58f3");

            var recruiter = new Recruiter
            {
                FirstName = recruiterForCreationDto.FirstName,
                LastName = recruiterForCreationDto.LastName,
                Position = recruiterForCreationDto.Position.ToRecruiterPosition(),
                DateOfBirth = recruiterForCreationDto.DateOfBirth,
                Email = recruiterForCreationDto.Email,
                Gender = recruiterForCreationDto.Gender
            };

            _repository.AddRecruiter(companyId, recruiter);
            if (_repository.Save())
            {
                return CreatedAtRoute("GetRecruiter", new { recruiterId = recruiter.Id }, recruiter);
            }
            else
            {
                _logger.LogInformation($"Create recruiter cannot be saved on repository");
                return BadRequest();
            }
        }

        [HttpPut("{recruiterId}")]
        public IActionResult UpdateRecruiter(Guid recruiterId, RecruiterForUpdateDto recruiterUpdate)
        {
            var recruiterFromRepo = _repository.GetRecruiter(recruiterId);

            if (recruiterFromRepo == null)
            {
                return NotFound();
            }

            recruiterFromRepo.FirstName = recruiterUpdate.FirstName;
            recruiterFromRepo.LastName = recruiterUpdate.LastName;
            recruiterFromRepo.Position = recruiterUpdate.Position.ToRecruiterPosition();
            recruiterFromRepo.DateOfBirth = recruiterUpdate.DateOfBirth;
            recruiterFromRepo.Email = recruiterUpdate.Email;
            recruiterFromRepo.Gender = recruiterUpdate.Gender;

            // Action without any effect
            _repository.UpdateRecruiter(recruiterFromRepo);

            _repository.Save();

            return NoContent();
        }

        [HttpPatch("{recruiterId}")]
        public ActionResult PartiallyUpdateRecruiter(Guid recruiterId, JsonPatchDocument<RecruiterForUpdateDto> patchDocument)
        {
            var recruiterFromRepo = _repository.GetRecruiter(recruiterId);

            if(recruiterFromRepo == null)
            {
                return NotFound();
            }

            var recruiterToPatch = new RecruiterForUpdateDto
            {
                FirstName = recruiterFromRepo.FirstName,
                LastName = recruiterFromRepo.LastName,
                Position = recruiterFromRepo.Position.ToString(),
                DateOfBirth = recruiterFromRepo.DateOfBirth,
                Email = recruiterFromRepo.Email,
                Gender = recruiterFromRepo.Gender
            };

            patchDocument.ApplyTo(recruiterToPatch, ModelState);

            if(!TryValidateModel(recruiterToPatch))
            {
                return ValidationProblem(ModelState);
            }

            recruiterFromRepo.FirstName = recruiterToPatch.FirstName;
            recruiterFromRepo.LastName = recruiterToPatch.LastName;
            recruiterFromRepo.Position = recruiterToPatch.Position.ToRecruiterPosition();
            recruiterFromRepo.DateOfBirth = recruiterToPatch.DateOfBirth;
            recruiterFromRepo.Email = recruiterToPatch.Email;
            recruiterFromRepo.Gender = recruiterToPatch.Gender;

            // Action without any effect
            _repository.UpdateRecruiter(recruiterFromRepo);

            _repository.Save();

            return NoContent();
        }

        [HttpDelete("{recruiterId}", Name = "DeleteRecruiter")]
        public ActionResult DeleteRecruiter(Guid recruiterId)
        {
            var recruiterFromRepo = _repository.GetRecruiter(recruiterId);

            if(recruiterFromRepo == null)
            {
                return NotFound();
            }

            _repository.DeleteRecruiter(recruiterFromRepo);

            _repository.Save();

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

        private IEnumerable<LinkDto> CreateLinksForRecruiters(RecruitersResourceParameters recruitersResourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(new LinkDto(CreateRecruitersResourceUri(recruitersResourceParameters, ResourceUriType.Current), "self", "GET"));
            if (hasNext)
            {
                links.Add(new LinkDto(CreateRecruitersResourceUri(recruitersResourceParameters, ResourceUriType.NextPage), "nextPage", "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateRecruitersResourceUri(recruitersResourceParameters, ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForRecruiter(Guid recruiterId, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetRecruiters", new { recruiterId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link("GetRecruiters", new { recruiterId, fields }),
                    "self",
                    "GET"));
            }
            links.Add(
                    new LinkDto(Url.Link("DeleteRecruiter", new { recruiterId }),
                    "delete_recruiter",
                    "DELETE"));
            links.Add(
                    new LinkDto(Url.Link("CreateRecruiter", new { recruiterId }),
                    "create_recruiter",
                    "POST"));
            return links;
        }

        private string CreateRecruitersResourceUri(RecruitersResourceParameters recruitersResourceParameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetRecruiters",
                        new
                        {
                            fields = recruitersResourceParameters.Fields,
                            pageNumber = recruitersResourceParameters.PageNumber - 1,
                            pageSize = recruitersResourceParameters.PageSize,
                            recruiterPosition = recruitersResourceParameters.RecruiterPosition,
                            companyId = recruitersResourceParameters.CompanyId,
                            orderBy = recruitersResourceParameters.OrderBy
                        }) ;
                case ResourceUriType.NextPage:
                    return Url.Link("GetRecruiters",
                        new
                        {
                            fields = recruitersResourceParameters.Fields,
                            pageNumber = recruitersResourceParameters.PageNumber + 1,
                            pageSize = recruitersResourceParameters.PageSize,
                            recruiterPosition = recruitersResourceParameters.RecruiterPosition,
                            companyId = recruitersResourceParameters.CompanyId,
                            orderBy = recruitersResourceParameters.OrderBy
                        });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetRecruiters",
                        new
                        {
                            fields = recruitersResourceParameters.Fields,
                            pageNumber = recruitersResourceParameters.PageNumber,
                            pageSize = recruitersResourceParameters.PageSize,
                            recruiterPosition = recruitersResourceParameters.RecruiterPosition,
                            companyId = recruitersResourceParameters.CompanyId,
                            orderBy = recruitersResourceParameters.OrderBy
                        });
            }
        }
    }
}
