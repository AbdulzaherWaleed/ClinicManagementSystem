using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ClinicManagement.Infrastructure.Identity;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User == null)
        {
            return Task.CompletedTask;
        }

        // Admin implicitly has all permissions
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Check if user has the requested permission claim
        var hasPermission = context.User.Claims.Any(c => c.Type == "Permission" && c.Value == requirement.Permission);
        
        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
