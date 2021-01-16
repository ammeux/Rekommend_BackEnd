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
    public class RekommendationsFilterAttribute : ResultFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            MediaTypeHeaderValue parsedMediaType = null;

            bool isParsedMediaTypeOk = false;
            if (context.HttpContext.Request.Headers.TryGetValue("Accept", out StringValues mediaType))
            {
                if (MediaTypeHeaderValue.TryParse(mediaType, out parsedMediaType))
                {
                    isParsedMediaTypeOk = true;
                }
            }
            var resultFromAction = context.Result as ObjectResult;
            if (resultFromAction?.Value == null
                || resultFromAction.StatusCode < 200
                || resultFromAction.StatusCode >= 300
                || !isParsedMediaTypeOk)
            {
                await next();
                return;
            }

            var rekommendationsFromRepo = (IPagedList<Rekommendation>)resultFromAction.Value;

            var paginationMetadata = new
            {
                totalCount = rekommendationsFromRepo.TotalItemCount,
                pageSize = rekommendationsFromRepo.PageSize,
                currentPage = rekommendationsFromRepo.PageNumber,
                totalPages = rekommendationsFromRepo.PageCount
            };

            context.HttpContext.Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            IEnumerable<RekommendationDto> rekommendations;

            IList<RekommendationDto> rekommendationsList = new List<RekommendationDto>();

            foreach (var rekommendation in rekommendationsFromRepo)
            {
                rekommendationsList.Add(rekommendation.ToDto());
            }

            rekommendations = rekommendationsList;

            IQueryCollection requestQuery = context.HttpContext.Request.Query;

            var shapedRekommendations = rekommendations.ShapeData(requestQuery["Fields"]);

            if (parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                var links = CreateLinksForRekommendations(requestQuery, rekommendationsFromRepo.HasNextPage, rekommendationsFromRepo.HasPreviousPage, context, rekommendationsFromRepo);
                var shapedRekommendationsWithLinks = shapedRekommendations.Select(rekommendations =>
                {
                    var rekommendationsAsDictionary = rekommendations as IDictionary<string, object>;
                    var rekommendationLinks = CreateLinksForRekommendation((Guid)rekommendationsAsDictionary["Id"], null, context);
                    rekommendationsAsDictionary.Add("links", rekommendationLinks);
                    return rekommendationsAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedRekommendationsWithLinks,
                    links
                };

                resultFromAction.Value = linkedCollectionResource;
            }
            else
            {
                resultFromAction.Value = rekommendations;
            }


            await next();
        }

        private IEnumerable<LinkDto> CreateLinksForRekommendations(IQueryCollection rekommendationsResourceParameters, bool hasNext, bool hasPrevious, ResultExecutingContext context, IPagedList<Rekommendation> rekommendations)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(new LinkDto(CreateRekommendationsResourceUri(rekommendationsResourceParameters, ResourceUriType.Current, context, rekommendations), "self", "GET"));
            if (hasNext)
            {
                links.Add(new LinkDto(CreateRekommendationsResourceUri(rekommendationsResourceParameters, ResourceUriType.NextPage, context, rekommendations), "nextPage", "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateRekommendationsResourceUri(rekommendationsResourceParameters, ResourceUriType.PreviousPage, context, rekommendations), "previousPage", "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForRekommendation(Guid rekommendationId, string fields, ResultExecutingContext context)
        {
            var links = new List<LinkDto>();

            var factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var contextAccessor = context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();

            var Url = factory.GetUrlHelper(contextAccessor.ActionContext);

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
                    new LinkDto(Url.Link("GetRekommendation", new { rekommendationId, fields }),
                    "self",
                    "GET"));
            }
            links.Add(
                    new LinkDto(Url.Link("DeleteRekommendation", new { rekommendationId }),
                    "delete_rekommendation",
                    "DELETE"));
            return links;
        }

        private string CreateRekommendationsResourceUri(IQueryCollection rekommendationsResourceParameters, ResourceUriType type, ResultExecutingContext context, IPagedList<Rekommendation> rekommendations)
        {
            var factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var contextAccessor = context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();

            var Url = factory.GetUrlHelper(contextAccessor.ActionContext);
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetRekommendations",
                        new
                        {
                            fields = rekommendationsResourceParameters["Fields"],
                            pageNumber = rekommendations.PageNumber - 1,
                            pageSize = rekommendations.PageSize,
                            techJobOpeningId = rekommendationsResourceParameters["TechJobOpeningId"],
                            position = rekommendationsResourceParameters["Position"],
                            seniority = rekommendationsResourceParameters["Seniority"],
                            rekommendationStatus = rekommendationsResourceParameters["Status"],
                            orderBy = rekommendationsResourceParameters["OrderBy"]
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetRekommendations",
                        new
                        {
                            fields = rekommendationsResourceParameters["Fields"],
                            pageNumber = rekommendations.PageNumber + 1,
                            pageSize = rekommendations.PageSize,
                            techJobOpeningId = rekommendationsResourceParameters["TechJobOpeningId"],
                            position = rekommendationsResourceParameters["Position"],
                            seniority = rekommendationsResourceParameters["Seniority"],
                            rekommendationStatus = rekommendationsResourceParameters["Status"],
                            orderBy = rekommendationsResourceParameters["OrderBy"]
                        });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetRekommendations",
                        new
                        {
                            fields = rekommendationsResourceParameters["Fields"],
                            pageNumber = rekommendations.PageNumber,
                            pageSize = rekommendations.PageSize,
                            techJobOpeningId = rekommendationsResourceParameters["TechJobOpeningId"],
                            position = rekommendationsResourceParameters["Position"],
                            seniority = rekommendationsResourceParameters["Seniority"],
                            rekommendationStatus = rekommendationsResourceParameters["Status"],
                            orderBy = rekommendationsResourceParameters["OrderBy"]
                        });
            }
        }
    }
}
