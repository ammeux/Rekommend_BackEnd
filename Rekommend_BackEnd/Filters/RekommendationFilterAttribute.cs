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
    public class RekommendationFilterAttribute : ResultFilterAttribute
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
               || resultFromAction.StatusCode >= 300 ||
               !isParsedMediaTypeOk)
            {
                await next();
                return;
            }

            var rekommendationFromRepo = (Rekommendation)resultFromAction.Value;

            RekommendationDto rekommendationDto = rekommendationFromRepo.ToDto();

            if (parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                string fields = context.HttpContext.Request.Query["Fields"];
                IEnumerable<LinkDto> links = CreateLinksForRekommendation(rekommendationDto.Id, fields, context);

                var rekommendationToReturn = rekommendationDto.ShapeData(fields) as IDictionary<string, object>;
                rekommendationToReturn.Add("links", links);

                resultFromAction.Value = rekommendationToReturn;
            }
            else
            {
                resultFromAction.Value = rekommendationDto;
            }
            await next();
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
                    new LinkDto(Url.Link("GetRekommendations", new { rekommendationId, fields }),
                    "self",
                    "GET"));
            }
            links.Add(
                    new LinkDto(Url.Link("DeleteRekommendation", new { rekommendationId }),
                    "delete_rekommendation",
                    "DELETE"));
            return links;
        }
    }
}
