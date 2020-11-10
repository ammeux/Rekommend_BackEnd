using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Rekommend_BackEnd.Extensions;
using Rekommend_BackEnd.Models;
using Rekommend_BackEnd.Repositories;
using Rekommend_BackEnd.ResourceParameters;
using Rekommend_BackEnd.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Controllers
{
    [ApiController]
    [Route("api/techJobOpenings")]
    public class TechJobOpeningsController : ControllerBase
    {
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly IRekommendRepository _repository;

        public TechJobOpeningsController(IRekommendRepository repository, IPropertyCheckerService propertyCheckerService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet(Name = "GetTechJobOpenings")]
        [HttpHead]
        public IActionResult GetTechJobOpenings([FromQuery] TechJobOpeningsResourceParameters techJobOpeningsResourceParameters, [FromHeader(Name ="Accept")] string mediaType)
        {
            if(!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if(!_propertyCheckerService.TypeHasProperties<TechJobOpeningDto>(techJobOpeningsResourceParameters.Fields))
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

            foreach(var techJobOpening in techJobOpeningsFromRepo)
            {
                techJobOpeningsList.Add(new TechJobOpeningDto()
                {
                    Id = techJobOpening.Id,
                    CreationDate = techJobOpening.CreationDate,
                    ClosingDate = techJobOpening.ClosingDate,
                    StartingDate = techJobOpening.StartingDate,
                    Title = techJobOpening.Title,
                    RecruiterId = techJobOpening.RecruiterId,
                    JobTechLanguage = techJobOpening.JobTechLanguage,
                    JobPosition = techJobOpening.JobPosition,
                    Seniority = techJobOpening.Seniority,
                    ContractType = techJobOpening.ContractType,
                    RemoteWorkAccepted = techJobOpening.RemoteWorkAccepted,
                    MissionDescription = techJobOpening.MissionDescription,
                    City = techJobOpening.City,
                    Country = techJobOpening.Country,
                    Reward1 = techJobOpening.Reward1,
                    Reward2 = techJobOpening.Reward2,
                    Reward3 = techJobOpening.Reward3,
                    LikesNb = techJobOpening.LikesNb,
                    RekommendationsNb = techJobOpening.RekommendationsNb,
                    ViewNb = techJobOpening.ViewsNb,
                    MinimumSalary = techJobOpening.MinimumSalary,
                    MaximumSalary = techJobOpening.MaximumSalary,
                    Status = techJobOpening.Status,
                    pictureFileName = techJobOpening.PictureFileName,
                    RseDescription = techJobOpening.RseDescription
                });
            }

            techJobOpenings = techJobOpeningsList;

            var shapedTechJobOpenings = techJobOpenings.ShapeData(techJobOpeningsResourceParameters.Fields);

            if(parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                var shapedTechJobOpeningsWithLinks = shapedTechJobOpenings.Select(techJobOpenings =>
                {
                    var techJobOpeningAsDictionary = techJobOpenings as IDictionary<string, object>;
                    var techJobOpeningLinks = CreateLinksForTechJobOpenings((Guid)techJobOpeningAsDictionary["Id"], null);
                    techJobOpeningAsDictionary.Add("links", techJobOpeningLinks);
                    return techJobOpeningAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedTechJobOpenings,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                return Ok(shapedTechJobOpenings);
            }
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

        private IEnumerable<LinkDto> CreateLinksForTechJobOpenings(Guid techJobOpeningId, string fields)
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
            //links.Add(
            //        new LinkDto(Url.Link("DeleteTechJobOpenings", new { techJobOpeningId }),
            //        "delete_techJobOpening",
            //        "DELETE"));
            //links.Add(
            //        new LinkDto(Url.Link("CreateTechJobOpenings", new { techJobOpeningId }),
            //        "create_techJobOpening",
            //        "POST"));
            return links;
        }

        private string CreateTechJobOpeningsResourceUri(TechJobOpeningsResourceParameters techJobOpeningsResourceParameters, ResourceUriType type)
        {
            switch(type)
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
