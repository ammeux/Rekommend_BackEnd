using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Models;
using System;
using System.Collections.Generic;
using Rekommend_BackEnd.Extensions;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Rekommend_BackEnd.Filters
{
    public class CompanyFilterAttribute : ResultFilterAttribute
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

            var companyFromRepo = (Company)resultFromAction.Value;

            CompanyDto companyDto = companyFromRepo.ToDto();

            if (parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                string fields = context.HttpContext.Request.Query["Fields"];
                IEnumerable<LinkDto> links = CreateLinksForCompany(companyDto.Id, fields, context);

                var companyToReturn = companyDto.ShapeData(fields) as IDictionary<string, object>;
                companyToReturn.Add("links", links);

                resultFromAction.Value = companyToReturn;
            }
            else
            {
                resultFromAction.Value = companyDto;
            }
            await next();
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(Guid companyId, string fields, ResultExecutingContext context)
        {
            var links = new List<LinkDto>();

            var factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var contextAccessor = context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();

            var Url = factory.GetUrlHelper(contextAccessor.ActionContext);

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetCompany", new { companyId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link("GetCompany", new { companyId, fields }),
                    "self",
                    "GET"));
            }
            links.Add(
                    new LinkDto(Url.Link("DeleteCompany", new { companyId }),
                    "delete_company",
                    "DELETE"));
            return links;
        }
    }
}
