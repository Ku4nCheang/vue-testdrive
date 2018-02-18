using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using netcore.Core.Constants;

namespace netcore.Core.Authentications
{

    //
    // ─── ANY ROLE POLICY ────────────────────────────────────────────────────────────
    //

    public class OwnerOrAnyRoleRequirement : IAuthorizationRequirement
    {
        public readonly string[] Roles;

        /// <summary>
        /// Create an authorization policy that only allow owner or specified roles to access.
        /// </summary>
        /// <param name="roles">Roles that are allowed to access whose values where separated by ','</param>
        public OwnerOrAnyRoleRequirement(string roles)
        {
            Roles = roles.Split(",");
        }
    }

    public class OwnerOrAnyRoleHandler : AuthorizationHandler<OwnerOrAnyRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerOrAnyRoleRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext mvcContext && context.User.Identity.Name == mvcContext.RouteData.Values["id"] as string)
            {
                context.Succeed(requirement);
            } 
            else
            {
                var isInRole = context.User
                    .FindAll(claim => claim.Type == JwtClaimTypes.Role)
                    .Select( c => c.Value )
                    .Any( role => requirement.Roles.Contains(role));

                if (isInRole)
                    context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    //
    // ─── SAME USER POLCY ────────────────────────────────────────────────────────────
    //

    public class SameUserRequirement : IAuthorizationRequirement {}

    public class SameUserHandler : AuthorizationHandler<SameUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext mvcContext)
            {
                if (context.User.Identity.Name == mvcContext.RouteData.Values["id"] as string) 
                {
                    context.Succeed(requirement);
                }
            }
            
            return Task.CompletedTask;
        }
    }
}