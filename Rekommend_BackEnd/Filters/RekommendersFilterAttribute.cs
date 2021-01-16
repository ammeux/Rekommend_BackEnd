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
    public class RekommendersFilterAttribute : ResultFilterAttribute
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

            var rekommendersFromRepo = (IPagedList<Rekommender>)resultFromAction.Value;

            var paginationMetadata = new
            {
                totalCount = rekommendersFromRepo.TotalItemCount,
                pageSize = rekommendersFromRepo.PageSize,
                currentPage = rekommendersFromRepo.PageNumber,
                totalPages = rekommendersFromRepo.PageCount
            };

            context.HttpContext.Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            IEnumerable<RekommenderDto> rekommenders;

            IList<RekommenderDto> rekommendersList = new List<RekommenderDto>();

            foreach (var rekommender in rekommendersFromRepo)
            {
                rekommendersList.Add(rekommender.ToDto());
            }

            rekommenders = rekommendersList;

            IQueryCollection requestQuery = context.HttpContext.Request.Query;

            var shapedRekommenders = rekommenders.ShapeData(requestQuery["Fields"]);

            if (parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                var links = CreateLinksForRekommenders(requestQuery, rekommendersFromRepo.HasNextPage, rekommendersFromRepo.HasPreviousPage, context, rekommendersFromRepo);
                var shapedRekommendersWithLinks = shapedRekommenders.Select(rekommenders =>
                {
                    var rekommendersAsDictionary = rekommenders as IDictionary<string, object>;
                    var rekommenderLinks = CreateLinksForRekommender((Guid)rekommendersAsDictionary["Id"], null, context);
                    rekommendersAsDictionary.Add("links", rekommenderLinks);
                    return rekommendersAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedRekommendersWithLinks,
                    links
                };

                resultFromAction.Value = linkedCollectionResource;
            }
            else
            {
                resultFromAction.Value = rekommenders;
            }


            await next();
        }

        private IEnumerable<LinkDto> CreateLinksForRekommenders(IQueryCollection rekommendersResourceParameters, bool hasNext, bool hasPrevious, ResultExecutingContext context, IPagedList<Rekommender> rekommenders)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(new LinkDto(CreateRekommendersResourceUri(rekommendersResourceParameters, ResourceUriType.Current, context, rekommenders), "self", "GET"));
            if (hasNext)
            {
                links.Add(new LinkDto(CreateRekommendersResourceUri(rekommendersResourceParameters, ResourceUriType.NextPage, context, rekommenders), "nextPage", "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateRekommendersResourceUri(rekommendersResourceParameters, ResourceUriType.PreviousPage, context, rekommenders), "previousPage", "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForRekommender(Guid rekommenderId, string fields, ResultExecutingContext context)
        {
            var links = new List<LinkDto>();

            var factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var contextAccessor = context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();

            var Url = factory.GetUrlHelper(contextAccessor.ActionContext);

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

        private string CreateRekommendersResourceUri(IQueryCollection rekommendersResourceParameters, ResourceUriType type, ResultExecutingContext context, IPagedList<Rekommender> rekommenders)
        {
            var factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var contextAccessor = context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();

            var Url = factory.GetUrlHelper(contextAccessor.ActionContext);
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetRekommenders",
                        new
                        {
                            fields = rekommendersResourceParameters["Fields"],
                            pageNumber = rekommenders.PageNumber - 1,
                            pageSize = rekommenders.PageSize,
                            position = rekommendersResourceParameters["Position"],
                            seniority = rekommendersResourceParameters["Seniority"],
                            xpRekommend = rekommendersResourceParameters["XpRekommend"],
                            RekommendationsAvgGrade = rekommendersResourceParameters["RekommendationsAvgGrade"],
                            level = rekommendersResourceParameters["Level"],
                            orderBy = rekommendersResourceParameters["OrderBy"]
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetRekommenders",
                        new
                        {
                            fields = rekommendersResourceParameters["Fields"],
                            pageNumber = rekommenders.PageNumber + 1,
                            pageSize = rekommenders.PageSize,
                            position = rekommendersResourceParameters["Position"],
                            seniority = rekommendersResourceParameters["Seniority"],
                            xpRekommend = rekommendersResourceParameters["XpRekommend"],
                            RekommendationsAvgGrade = rekommendersResourceParameters["RekommendationsAvgGrade"],
                            level = rekommendersResourceParameters["Level"],
                            orderBy = rekommendersResourceParameters["OrderBy"]
                        });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetRekommenders",
                        new
                        {
                            fields = rekommendersResourceParameters["Fields"],
                            pageNumber = rekommenders.PageNumber,
                            pageSize = rekommenders.PageSize,
                            position = rekommendersResourceParameters["Position"],
                            seniority = rekommendersResourceParameters["Seniority"],
                            xpRekommend = rekommendersResourceParameters["XpRekommend"],
                            RekommendationsAvgGrade = rekommendersResourceParameters["RekommendationsAvgGrade"],
                            level = rekommendersResourceParameters["Level"],
                            orderBy = rekommendersResourceParameters["OrderBy"]
                        });
            }
        }
    }
}
