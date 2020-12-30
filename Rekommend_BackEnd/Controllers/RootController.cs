using Microsoft.AspNetCore.Mvc;
using Rekommend_BackEnd.Models;
using System.Collections.Generic;

namespace Rekommend_BackEnd.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        [Produces("application/json", "application/vnd.rekom.hateoas+json")]
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            // create links for root
            var links = new List<LinkDto>();

            links.Add(
              new LinkDto(Url.Link("GetRoot", new { }),
              "self",
              "GET"));

            //TechJobOpenings
            links.Add(
              new LinkDto(Url.Link("GetTechJobOpenings", new { }),
              "techJobOpenings",
              "GET"));
            links.Add(
              new LinkDto(Url.Link("CreateTechJobOpening", new { }),
              "create_techJobOpening",
              "POST"));

            //Companies
            links.Add(
              new LinkDto(Url.Link("GetCompanies", new { }),
              "companies",
              "GET"));
            links.Add(
              new LinkDto(Url.Link("CreateCompany", new { }),
              "create_company",
              "POST"));

            //Recruiters
            links.Add(
              new LinkDto(Url.Link("GetRecruiters", new { }),
              "recruiters",
              "GET"));
            links.Add(
              new LinkDto(Url.Link("CreateRecruiter", new { }),
              "create_recruiter",
              "POST"));

            //Rekommendations
            links.Add(
              new LinkDto(Url.Link("GetRekommendations", new { }),
              "rekommendations",
              "GET"));
            links.Add(
              new LinkDto(Url.Link("CreateRekommendation", new { }),
              "create_rekommendation",
              "POST"));

            //Rekommenders
            links.Add(
              new LinkDto(Url.Link("GetRekommenders", new { }),
              "rekommenders",
              "GET"));
            links.Add(
              new LinkDto(Url.Link("CreateRekommender", new { }),
              "create_rekommender",
              "POST"));

            return Ok(links);
        }
    }
}
