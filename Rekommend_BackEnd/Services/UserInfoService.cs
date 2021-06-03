using Microsoft.AspNetCore.Http;
using Rekommend_BackEnd.Extensions;
using System;
using System.Linq;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Services
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public Country? Country { get; set; }
        public string Company { get; set; }
        public Profile? Profile { get; set; }
        public Seniority? Seniority { get; set; }
        public JobTechLanguage? Stack { get; set; }

        public UserInfoService(IHttpContextAccessor httpContextAccessor)
        {
            // service is scoped, created once for each request => we only need
            // to fetch the info in the constructor
            _httpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            var currentContext = _httpContextAccessor.HttpContext;
            if (currentContext == null || !currentContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            if(Guid.TryParse(currentContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value, out Guid id))
            {
                UserId = id;
            }
            FirstName = currentContext.User.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
            LastName = currentContext.User.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value;
            Email = currentContext.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            Address = currentContext.User.Claims.FirstOrDefault(c => c.Type == "address")?.Value;
            Country = currentContext.User.Claims.FirstOrDefault(c => c.Type == "country")?.Value.ToCountry();
            Company = currentContext.User.Claims.FirstOrDefault(c => c.Type == "company")?.Value;
            Profile = currentContext.User.Claims.FirstOrDefault(c => c.Type == "profile")?.Value.ToProfile();
            Seniority = currentContext.User.Claims.FirstOrDefault(c => c.Type == "seniority")?.Value.ToSeniority();
            Stack = currentContext.User.Claims.FirstOrDefault(c => c.Type == "stack")?.Value.ToJobTechLanguage();
        }
    }
}
