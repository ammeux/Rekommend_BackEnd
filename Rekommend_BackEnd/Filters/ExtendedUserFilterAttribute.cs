using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Rekommend_BackEnd.Extensions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Rekommend_BackEnd.Filters
{
    public class ExtendedUserFilterAttribute : ResultFilterAttribute
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

            var extendedUserFromRepo = (ExtendedUser)resultFromAction.Value;

            ExtendedUserDto extendedUserDto = extendedUserFromRepo.ToDto();

            context.HttpContext.Request.Headers.TryGetValue("Accept", out StringValues mediaType);

            if (MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType) && parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                string fields = context.HttpContext.Request.Query["Fields"];
                IEnumerable<LinkDto> links = CreateLinksForExtendedUsers(extendedUserDto.Id, fields, context);

                var extendedUserToReturn = extendedUserDto.ShapeData(fields) as IDictionary<string, object>;
                extendedUserToReturn.Add("links", links);

                resultFromAction.Value = extendedUserToReturn;
            }
            else
            {
                resultFromAction.Value = extendedUserDto;
            }
            await next();
        }

        private IEnumerable<LinkDto> CreateLinksForExtendedUsers(Guid extendedUserId, string fields, ResultExecutingContext context)
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
    }
}
