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
    public class RecruiterFilterAttribute : ResultFilterAttribute
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

            var recruiterFromRepo = (Recruiter)resultFromAction.Value;

            RecruiterDto recruiterDto = recruiterFromRepo.ToDto();

            if (parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                string fields = context.HttpContext.Request.Query["Fields"];
                IEnumerable<LinkDto> links = CreateLinksForRecruiter(recruiterDto.Id, fields, context);

                var recruiterToReturn = recruiterDto.ShapeData(fields) as IDictionary<string, object>;
                recruiterToReturn.Add("links", links);

                resultFromAction.Value = recruiterToReturn;
            }
            else
            {
                resultFromAction.Value = recruiterDto;
            }
            await next();
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
    }
}
