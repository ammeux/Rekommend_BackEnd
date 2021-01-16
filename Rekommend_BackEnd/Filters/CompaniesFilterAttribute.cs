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
    public class CompaniesFilterAttribute : ResultFilterAttribute
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

            var companiesFromRepo = (IPagedList<Company>)resultFromAction.Value;

            var paginationMetadata = new
            {
                totalCount = companiesFromRepo.TotalItemCount,
                pageSize = companiesFromRepo.PageSize,
                currentPage = companiesFromRepo.PageNumber,
                totalPages = companiesFromRepo.PageCount
            };

            context.HttpContext.Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            IEnumerable<CompanyDto> companies;

            IList<CompanyDto> companiesList = new List<CompanyDto>();

            foreach (var company in companiesFromRepo)
            {
                companiesList.Add(company.ToDto());
            }

            companies = companiesList;

            IQueryCollection requestQuery = context.HttpContext.Request.Query;

            var shapedCompanies = companies.ShapeData(requestQuery["Fields"]);

            if (parsedMediaType.MediaType == "application/vnd.rekom.hateoas+json")
            {
                var links = CreateLinksForCompanies(requestQuery, companiesFromRepo.HasNextPage, companiesFromRepo.HasPreviousPage, context, companiesFromRepo);
                var shapedCompaniesWithLinks = shapedCompanies.Select(companies =>
                {
                    var companiesAsDictionary = companies as IDictionary<string, object>;
                    var companyLinks = CreateLinksForCompany((Guid)companiesAsDictionary["Id"], null, context);
                    companiesAsDictionary.Add("links", companyLinks);
                    return companiesAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedCompaniesWithLinks,
                    links
                };

                resultFromAction.Value = linkedCollectionResource;
            }
            else
            {
                resultFromAction.Value = companies;
            }


            await next();
        }

        private IEnumerable<LinkDto> CreateLinksForCompanies(IQueryCollection companiesResourceParameters, bool hasNext, bool hasPrevious, ResultExecutingContext context, IPagedList<Company> companies)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(new LinkDto(CreateCompaniesResourceUri(companiesResourceParameters, ResourceUriType.Current, context, companies), "self", "GET"));
            if (hasNext)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(companiesResourceParameters, ResourceUriType.NextPage, context, companies), "nextPage", "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(companiesResourceParameters, ResourceUriType.PreviousPage, context, companies), "previousPage", "GET"));
            }

            return links;
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

        private string CreateCompaniesResourceUri(IQueryCollection companiesResourceParameters, ResourceUriType type, ResultExecutingContext context, IPagedList<Company> companies)
        {
            var factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var contextAccessor = context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();

            var Url = factory.GetUrlHelper(contextAccessor.ActionContext);
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCompanies",
                        new
                        {
                            fields = companiesResourceParameters["Fields"],
                            pageNumber = companies.PageNumber - 1,
                            pageSize = companies.PageSize,
                            name = companiesResourceParameters["Name"],
                            hqCity = companiesResourceParameters["HqCity"],
                            hqCountry = companiesResourceParameters["HqCountry"],
                            category = companiesResourceParameters["Category"],
                            orderBy = companiesResourceParameters["OrderBy"]
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCompanies",
                        new
                        {
                            fields = companiesResourceParameters["Fields"],
                            pageNumber = companies.PageNumber + 1,
                            pageSize = companies.PageSize,
                            name = companiesResourceParameters["Name"],
                            hqCity = companiesResourceParameters["HqCity"],
                            hqCountry = companiesResourceParameters["HqCountry"],
                            category = companiesResourceParameters["Category"],
                            orderBy = companiesResourceParameters["OrderBy"]
                        });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetCompanies",
                        new
                        {
                            fields = companiesResourceParameters["Fields"],
                            pageNumber = companies.PageNumber,
                            pageSize = companies.PageSize,
                            name = companiesResourceParameters["Name"],
                            hqCity = companiesResourceParameters["HqCity"],
                            hqCountry = companiesResourceParameters["HqCountry"],
                            category = companiesResourceParameters["Category"],
                            orderBy = companiesResourceParameters["OrderBy"]
                        });
            }
        }
    }
}
