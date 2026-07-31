using System.Security.Claims;
using ClinicManagement.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ClinicManagement.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var guid) ? guid : null;
        }
    }

    public string? Role => _httpContextAccessor.HttpContext?.User?
        .FindFirstValue(ClaimTypes.Role);

    public Guid? AssignedDoctorId
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?.User?
                .FindFirstValue("doctorId");
            return Guid.TryParse(id, out var guid) ? guid : null;
        }
    }
}