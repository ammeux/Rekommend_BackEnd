using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rekommend_BackEnd.Services
{
    public interface IUserInfoService
    {
        string UserId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
    }
}
