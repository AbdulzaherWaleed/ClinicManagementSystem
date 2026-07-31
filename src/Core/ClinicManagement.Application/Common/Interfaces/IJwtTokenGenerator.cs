using ClinicManagement.Domain.Entities.Identity;

namespace ClinicManagement.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
}