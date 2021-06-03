using Microsoft.AspNetCore.Authorization;

namespace Rekommend_BackEnd.Authorization
{
    public class MustBeAnExtendedUserRequirement : IAuthorizationRequirement
    {
        public MustBeAnExtendedUserRequirement()
        {

        }
    }
}
