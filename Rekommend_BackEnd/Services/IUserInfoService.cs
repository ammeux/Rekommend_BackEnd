using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Services
{
    public interface IUserInfoService
    {
        Guid UserId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string City { get; set; }
        Country? Country { get; set; }
        string Company { get; set; }
        Profile? Profile { get; set; }
        Seniority? Seniority { get; set; }
        JobTechLanguage? Stack { get; set; }
    }
}
