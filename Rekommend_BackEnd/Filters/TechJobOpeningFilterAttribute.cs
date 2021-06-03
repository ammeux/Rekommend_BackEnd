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
    public class TechJobOpeningFilterAttribute : ResultFilterAttribute
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

            var techJobOpeningFromRepo = (TechJobOpening)resultFromAction.Value;

            TechJobOpeningDto techJobOpeningDto = techJobOpeningFromRepo.ToDto();

            context.HttpContext.Request.Headers.TryGetValue("Accept", out StringValues mediaType);

            if (MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType) && parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                string fields = context.HttpContext.Request.Query["Fields"];
                IEnumerable<LinkDto> links = CreateLinksForTechJobOpening(techJobOpeningDto.Id, fields, context);

                var techJobOpeningToReturn = techJobOpeningDto.ShapeData(fields) as IDictionary<string, object>;
                techJobOpeningToReturn.Add("links", links);

                resultFromAction.Value = techJobOpeningToReturn;
            }
            else
            {
                resultFromAction.Value = techJobOpeningDto;
            }
            await next();
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
    } 
}
