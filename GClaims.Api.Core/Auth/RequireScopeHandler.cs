using Microsoft.AspNetCore.Authorization;

namespace GClaims.Core.Auth
{
    public class RequireScopeHandler : AuthorizationHandler<ScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
        {
            // The scope must have originated from our issuer.
            var scopeClaim = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer);
            if (scopeClaim == null || String.IsNullOrEmpty(scopeClaim.Value))
            {
                return Task.CompletedTask;
            }

            // A token can contain multiple scopes and we need at least one exact match.
            if (scopeClaim.Value.Split(' ').Any(s => s == requirement.Scope))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
