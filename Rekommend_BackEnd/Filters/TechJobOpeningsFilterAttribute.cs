using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using PagedList;
using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Extensions;
using Rekommend_BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Filters
{
    public class TechJobOpeningsFilterAttribute : ResultFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var resultFromAction = context.Result as ObjectResult;
            if (resultFromAction?.Value == null
                || resultFromAction.StatusCode < 200
                || resultFromAction.StatusCode >= 300)
            {
                await next();
                return;
            }

            var techJobOpeningsFromRepo = (IPagedList<TechJobOpening>)resultFromAction.Value;

            var paginationMetadata = new
            {
                totalCount = techJobOpeningsFromRepo.TotalItemCount,
                pageSize = techJobOpeningsFromRepo.PageSize,
                currentPage = techJobOpeningsFromRepo.PageNumber,
                totalPages = techJobOpeningsFromRepo.PageCount
            };

            context.HttpContext.Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            IEnumerable<TechJobOpeningDto> techJobOpenings;

            IList<TechJobOpeningDto> techJobOpeningsList = new List<TechJobOpeningDto>();

            foreach (var techJobOpening in techJobOpeningsFromRepo)
            {
                techJobOpeningsList.Add(techJobOpening.ToDto());
            }

            techJobOpenings = techJobOpeningsList;

            IQueryCollection requestQuery = context.HttpContext.Request.Query;

            var shapedTechJobOpenings = techJobOpenings.ShapeData(requestQuery["Fields"]);

            context.HttpContext.Request.Headers.TryGetValue("Accept", out StringValues mediaType);

            if (MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType) && parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                var links = CreateLinksForTechJobOpenings(requestQuery, techJobOpeningsFromRepo.HasNextPage, techJobOpeningsFromRepo.HasPreviousPage, context, techJobOpeningsFromRepo);
                var shapedTechJobOpeningsWithLinks = shapedTechJobOpenings.Select(techJobOpenings =>
                {
                    var techJobOpeningAsDictionary = techJobOpenings as IDictionary<string, object>;
                    var techJobOpeningLinks = CreateLinksForTechJobOpening((Guid)techJobOpeningAsDictionary["Id"], null, context);
                    techJobOpeningAsDictionary.Add("links", techJobOpeningLinks);
                    return techJobOpeningAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedTechJobOpeningsWithLinks,
                    links
                };

                resultFromAction.Value = linkedCollectionResource;
            }
            else
            {
                resultFromAction.Value = shapedTechJobOpenings;
            }

            await next();
        }

        private IEnumerable<LinkDto> CreateLinksForTechJobOpenings(IQueryCollection techJobOpeningsResourceParameters, bool hasNext, bool hasPrevious, ResultExecutingContext context, IPagedList<TechJobOpening> techJobOpenings)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(new LinkDto(CreateTechJobOpeningsResourceUri(techJobOpeningsResourceParameters, ResourceUriType.Current, context, techJobOpenings), "self", "GET"));
            if (hasNext)
            {
                links.Add(new LinkDto(CreateTechJobOpeningsResourceUri(techJobOpeningsResourceParameters, ResourceUriType.NextPage, context, techJobOpenings), "nextPage", "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateTechJobOpeningsResourceUri(techJobOpeningsResourceParameters, ResourceUriType.PreviousPage, context, techJobOpenings), "previousPage", "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForTechJobOpening(Guid techJobOpeningId, string fields, ResultExecutingContext context)
        {
            var links = new List<LinkDto>();

            var factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var contextAccessor = context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();

            var Url = factory.GetUrlHelper(contextAccessor.ActionContext);

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetTechJobOpening", new { techJobOpeningId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link("GetTechJobOpening", new { techJobOpeningId, fields }),
                    "self",
                    "GET"));
            }
            links.Add(
                    new LinkDto(Url.Link("DeleteTechJobOpening", new { techJobOpeningId }),
                    "delete_techJobOpening",
                    "DELETE"));
            return links;
        }

        private string CreateTechJobOpeningsResourceUri(IQueryCollection techJobOpeningsResourceParameters, ResourceUriType type, ResultExecutingContext context, IPagedList<TechJobOpening> techJobOpenings)
        {
            var factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var contextAccessor = context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();

            var Url = factory.GetUrlHelper(contextAccessor.ActionContext);
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetTechJobOpenings",
                        new
                        {
                            fields = techJobOpeningsResourceParameters["Fields"],
                            orderBy = techJobOpeningsResourceParameters["OrderBy"],
                            pageNumber = techJobOpenings.PageNumber - 1,
                            pageSize = techJobOpenings.PageSize,
                            companyCategory = techJobOpeningsResourceParameters["CompanyCategory"],
                            city = techJobOpeningsResourceParameters["City"],
                            remoteWorkAccepted = techJobOpeningsResourceParameters["RemoteWorkAccepted"],
                            contractType = techJobOpeningsResourceParameters["ContractType"]
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetTechJobOpenings",
                        new
                        {
                            fields = techJobOpeningsResourceParameters["Fields"],
                            orderBy = techJobOpeningsResourceParameters["OrderBy"],
                            pageNumber = techJobOpenings.PageNumber + 1,
                            pageSize = techJobOpenings.PageSize,
                            companyCategory = techJobOpeningsResourceParameters["CompanyCategory"],
                            city = techJobOpeningsResourceParameters["City"],
                            remoteWorkAccepted = techJobOpeningsResourceParameters["RemoteWorkAccepted"],
                            contractType = techJobOpeningsResourceParameters["ContractType"]
                        });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetTechJobOpenings",
                        new
                        {
                            fields = techJobOpeningsResourceParameters["Fields"],
                            orderBy = techJobOpeningsResourceParameters["OrderBy"],
                            pageNumber = techJobOpenings.PageNumber,
                            pageSize = techJobOpenings.PageSize,
                            companyCategory = techJobOpeningsResourceParameters["CompanyCategory"],
                            city = techJobOpeningsResourceParameters["City"],
                            remoteWorkAccepted = techJobOpeningsResourceParameters["RemoteWorkAccepted"],
                            contractType = techJobOpeningsResourceParameters["ContractType"]
                        });
            }
        }
    }
}
