using ClinicManagement.Domain.Entities.Identity;
using System.Security.Claims;

namespace ClinicManagement.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user, IList<string> roles, IList<Claim> additionalClaims);
    string GenerateRefreshToken();
}