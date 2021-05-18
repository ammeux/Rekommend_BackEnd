using Microsoft.AspNetCore.Authorization;
using Rekommend_BackEnd.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rekommend_BackEnd.Authorization
{
    public class MustBeARecruiterHandler : AuthorizationHandler<MustBeARecruiterRequirement>
    {
        private readonly IRekommendRepository _rekommendRepository;

        public MustBeARecruiterHandler(IRekommendRepository rekommendRepository)
        {
            _rekommendRepository = rekommendRepository ?? throw new ArgumentNullException(nameof(rekommendRepository));
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustBeARecruiterRequirement requirement)
        {

            var ownerId = context.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            
            if (!Guid.TryParse(ownerId, out Guid ownerIdAsGuid) || !_rekommendRepository.IsAuthorizedToPublish(ownerIdAsGuid))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            // all checks out
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
