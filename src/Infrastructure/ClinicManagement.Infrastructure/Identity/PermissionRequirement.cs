using ClinicManagement.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace ClinicManagement.Infrastructure.Identity;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
