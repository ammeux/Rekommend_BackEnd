using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using PagedList;
using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Rekommend_BackEnd.Extensions;
using Microsoft.AspNetCore.Http;
using static Rekommend_BackEnd.Utils.RekomEnums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Rekommend_BackEnd.Filters
{
    public class RecruitersFilterAttribute : ResultFilterAttribute
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

            var recruitersFromRepo = (IPagedList<Recruiter>)resultFromAction.Value;

            var paginationMetadata = new
            {
                totalCount = recruitersFromRepo.TotalItemCount,
                pageSize = recruitersFromRepo.PageSize,
                currentPage = recruitersFromRepo.PageNumber,
                totalPages = recruitersFromRepo.PageCount
            };

            context.HttpContext.Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            IEnumerable<RecruiterDto> recruiters;

            IList<RecruiterDto> recruitersList = new List<RecruiterDto>();

            foreach (var recruiter in recruitersFromRepo)
            {
                recruitersList.Add(recruiter.ToDto());
            }

            recruiters = recruitersList;

            IQueryCollection requestQuery = context.HttpContext.Request.Query;

            var shapedRecruiters = recruiters.ShapeData(requestQuery["Fields"]);

            if (parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                var links = CreateLinksForRecruiters(requestQuery, recruitersFromRepo.HasNextPage, recruitersFromRepo.HasPreviousPage, context, recruitersFromRepo);
                var shapedRecruitersWithLinks = shapedRecruiters.Select(recruiters =>
                {
                    var recruiterAsDictionary = recruiters as IDictionary<string, object>;
                    var recruiterLinks = CreateLinksForRecruiter((Guid)recruiterAsDictionary["Id"], null, context);
                    recruiterAsDictionary.Add("links", recruiterLinks);
                    return recruiterAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedRecruitersWithLinks,
                    links
                };

                resultFromAction.Value = linkedCollectionResource;
            }
            else
            {
                resultFromAction.Value = recruiters;
            }


            await next();
        }

        private IEnumerable<LinkDto> CreateLinksForRecruiters(IQueryCollection recruitersResourceParameters, bool hasNext, bool hasPrevious, ResultExecutingContext context, IPagedList<Recruiter> recruiters)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(new LinkDto(CreateRecruitersResourceUri(recruitersResourceParameters, ResourceUriType.Current, context, recruiters), "self", "GET"));
            if (hasNext)
            {
                links.Add(new LinkDto(CreateRecruitersResourceUri(recruitersResourceParameters, ResourceUriType.NextPage, context, recruiters), "nextPage", "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateRecruitersResourceUri(recruitersResourceParameters, ResourceUriType.PreviousPage, context, recruiters), "previousPage", "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForRecruiter(Guid recruiterId, string fields, ResultExecutingContext context)
        {
            var links = new List<LinkDto>();

            var factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var contextAccessor = context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();

            var Url = factory.GetUrlHelper(contextAccessor.ActionContext);

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetRecruiter", new { recruiterId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link("GetRecruiter", new { recruiterId, fields }),
                    "self",
                    "GET"));
            }
            links.Add(
                    new LinkDto(Url.Link("DeleteRecruiter", new { recruiterId }),
                    "delete_recruiter",
                    "DELETE"));
            return links;
        }

        private string CreateRecruitersResourceUri(IQueryCollection recruitersResourceParameters, ResourceUriType type, ResultExecutingContext context, IPagedList<Recruiter> recruiters)
        {
            var factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var contextAccessor = context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();

            var Url = factory.GetUrlHelper(contextAccessor.ActionContext);
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetRecruiters",
                        new
                        {
                            fields = recruitersResourceParameters["Fields"],
                            orderBy = recruitersResourceParameters["OrderBy"],
                            pageNumber = recruiters.PageNumber - 1,
                            pageSize = recruiters.PageSize,
                            recruiterPosition = recruitersResourceParameters["RecruiterPosition"],
                            companyId = recruitersResourceParameters["CompanyId"]
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetRecruiters",
                        new
                        {
                            fields = recruitersResourceParameters["Fields"],
                            orderBy = recruitersResourceParameters["OrderBy"],
                            pageNumber = recruiters.PageNumber + 1,
                            pageSize = recruiters.PageSize,
                            recruiterPosition = recruitersResourceParameters["RecruiterPosition"],
                            companyId = recruitersResourceParameters["CompanyId"]
                        });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetRecruiters",
                        new
                        {
                            fields = recruitersResourceParameters["Fields"],
                            orderBy = recruitersResourceParameters["OrderBy"],
                            pageNumber = recruiters.PageNumber,
                            pageSize = recruiters.PageSize,
                            recruiterPosition = recruitersResourceParameters["RecruiterPosition"],
                            companyId = recruitersResourceParameters["CompanyId"]
                        });
            }
        }
    }
}
