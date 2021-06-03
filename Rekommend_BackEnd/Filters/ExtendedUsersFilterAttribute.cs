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
    public class ExtendedUsersFilterAttribute : ResultFilterAttribute
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

            var extendedUsersFromRepo = (IPagedList<ExtendedUser>)resultFromAction.Value;

            var paginationMetadata = new
            {
                totalCount = extendedUsersFromRepo.TotalItemCount,
                pageSize = extendedUsersFromRepo.PageSize,
                currentPage = extendedUsersFromRepo.PageNumber,
                totalPages = extendedUsersFromRepo.PageCount
            };

            context.HttpContext.Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            IEnumerable<ExtendedUserDto> extendedUsers;

            IList<ExtendedUserDto> extendedUsersList = new List<ExtendedUserDto>();

            foreach (var recruiter in extendedUsersFromRepo)
            {
                extendedUsersList.Add(recruiter.ToDto());
            }

            extendedUsers = extendedUsersList;

            IQueryCollection requestQuery = context.HttpContext.Request.Query;

            var shapedExtendedUsers = extendedUsers.ShapeData(requestQuery["Fields"]);

            context.HttpContext.Request.Headers.TryGetValue("Accept", out StringValues mediaType);

            if (MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType) && parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                var links = CreateLinksForRecruiters(requestQuery, extendedUsersFromRepo.HasNextPage, extendedUsersFromRepo.HasPreviousPage, context, extendedUsersFromRepo);
                var shapedRecruitersWithLinks = shapedExtendedUsers.Select(recruiters =>
                {
                    var recruiterAsDictionary = recruiters as IDictionary<string, object>;
                    var recruiterLinks = CreateLinksForExtendedUser((Guid)recruiterAsDictionary["Id"], null, context);
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
                resultFromAction.Value = shapedExtendedUsers;
            }

            await next();
        }

        private IEnumerable<LinkDto> CreateLinksForRecruiters(IQueryCollection recruitersResourceParameters, bool hasNext, bool hasPrevious, ResultExecutingContext context, IPagedList<ExtendedUser> recruiters)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(new LinkDto(CreateExtendedUsersResourceUri(recruitersResourceParameters, ResourceUriType.Current, context, recruiters), "self", "GET"));
            if (hasNext)
            {
                links.Add(new LinkDto(CreateExtendedUsersResourceUri(recruitersResourceParameters, ResourceUriType.NextPage, context, recruiters), "nextPage", "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateExtendedUsersResourceUri(recruitersResourceParameters, ResourceUriType.PreviousPage, context, recruiters), "previousPage", "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForExtendedUser(Guid extendedUserId, string fields, ResultExecutingContext context)
        {
            var links = new List<LinkDto>();

            var factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var contextAccessor = context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();

            var Url = factory.GetUrlHelper(contextAccessor.ActionContext);

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetExtendedUser", new { extendedUserId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link("GetExtendedUser", new { extendedUserId, fields }),
                    "self",
                    "GET"));
            }
            links.Add(
                    new LinkDto(Url.Link("DeleteExtendedUser", new { extendedUserId }),
                    "delete_extendedUser",
                    "DELETE"));
            return links;
        }

        private string CreateExtendedUsersResourceUri(IQueryCollection extendedUsersResourceParameters, ResourceUriType type, ResultExecutingContext context, IPagedList<ExtendedUser> extendedUsers)
        {
            var factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var contextAccessor = context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();

            var Url = factory.GetUrlHelper(contextAccessor.ActionContext);
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetExtendedUsers",
                        new
                        {
                            fields = extendedUsersResourceParameters["Fields"],
                            orderBy = extendedUsersResourceParameters["OrderBy"],
                            pageNumber = extendedUsers.PageNumber - 1,
                            pageSize = extendedUsers.PageSize,
                            recruiterPosition = extendedUsersResourceParameters["ExtendedUserPosition"],
                            companyId = extendedUsersResourceParameters["CompanyId"]
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetExtendedUsers",
                        new
                        {
                            fields = extendedUsersResourceParameters["Fields"],
                            orderBy = extendedUsersResourceParameters["OrderBy"],
                            pageNumber = extendedUsers.PageNumber + 1,
                            pageSize = extendedUsers.PageSize,
                            recruiterPosition = extendedUsersResourceParameters["ExtendedUserPosition"],
                            companyId = extendedUsersResourceParameters["CompanyId"]
                        });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetExtendedUsers",
                        new
                        {
                            fields = extendedUsersResourceParameters["Fields"],
                            orderBy = extendedUsersResourceParameters["OrderBy"],
                            pageNumber = extendedUsers.PageNumber,
                            pageSize = extendedUsers.PageSize,
                            recruiterPosition = extendedUsersResourceParameters["ExtendedUserPosition"],
                            companyId = extendedUsersResourceParameters["CompanyId"]
                        });
            }
        }
    }
}
