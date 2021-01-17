using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Extensions;
using Rekommend_BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Rekommend_BackEnd.Filters
{
    public class RekommenderFilterAttribute : ResultFilterAttribute
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

            var rekommenderFromRepo = (Rekommender)resultFromAction.Value;

            RekommenderDto rekommenderDto = rekommenderFromRepo.ToDto();

            context.HttpContext.Request.Headers.TryGetValue("Accept", out StringValues mediaType);

            if (MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType) && parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                string fields = context.HttpContext.Request.Query["Fields"];
                IEnumerable<LinkDto> links = CreateLinksForRekommender(rekommenderDto.Id, fields, context);

                var rekommenderToReturn = rekommenderDto.ShapeData(fields) as IDictionary<string, object>;
                rekommenderToReturn.Add("links", links);

                resultFromAction.Value = rekommenderToReturn;
            }
            else
            {
                resultFromAction.Value = rekommenderDto;
            }
            await next();
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
    }
}
