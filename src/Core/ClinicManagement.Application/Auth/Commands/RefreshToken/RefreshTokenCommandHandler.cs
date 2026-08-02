using ClinicManagement.Application.Auth.DTOs;
using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClinicManagement.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IApplicationDbContext _context;

    public RefreshTokenCommandHandler(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator, IApplicationDbContext context)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _context = context;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = GetPrincipalFromExpiredToken(request.Token);
        if (principal == null)
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("Token", "Invalid access token or refresh token")
            });

        var userIdString = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("Token", "Invalid access token or refresh token")
            });

        var user = await _userManager.FindByIdAsync(userIdString);
        if (user == null || !user.IsActive)
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("Token", "Invalid access token or refresh token")
            });

        var userRefreshToken = await _context.UserRefreshTokens
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken && x.UserId == userId, cancellationToken);

        if (userRefreshToken == null || !userRefreshToken.IsActive)
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("Token", "Invalid access token or refresh token")
            });

        // Revoke the old refresh token
        userRefreshToken.RevokedAt = DateTime.UtcNow;
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        userRefreshToken.ReplacedByToken = newRefreshToken;

        var newRefreshTokenEntity = new UserRefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.UserRefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user, roles);

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = newRefreshToken,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "",
                AssignedDoctorIds = user.AssignedDoctorIds
            }
        };
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        // This relies on the IJwtTokenGenerator or manual JWT extraction,
        // but since we need the validation parameters, let's just use manual extraction
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims, "Bearer");
            return new ClaimsPrincipal(identity);
        }
        catch
        {
            return null;
        }
    }
}
