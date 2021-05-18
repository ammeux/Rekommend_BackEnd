using Microsoft.AspNetCore.Authorization;

namespace Rekommend_BackEnd.Authorization
{
    public class MustBeARecruiterRequirement : IAuthorizationRequirement
    {
        public MustBeARecruiterRequirement()
        {

        }
    }
}
