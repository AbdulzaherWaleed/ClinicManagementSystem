using ClinicManagement.Application.Auth.DTOs;
using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClinicManagement.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IApplicationDbContext _context;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager, 
        IJwtTokenGenerator jwtTokenGenerator,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _context = context;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.IsActive)
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("Email", "Invalid email or password.")
            });

        if (await _userManager.IsLockedOutAsync(user))
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("Email", "Account is temporarily locked due to too many failed attempts. Please try again later.")
            });

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user);
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("Password", "Invalid email or password.")
            });
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        // v2.1 — Generate JWT with multiple "doctorId" claims (one per assigned doctor)
        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        var userRefreshToken = new UserRefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7) // e.g., 7 days refresh token validity
        };

        _context.UserRefreshTokens.Add(userRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Update LastLoginAt
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "",
                AssignedDoctorIds = user.AssignedDoctorIds // List<Guid> parsed from AssignedDoctorIdsRaw
            }
        };
    }
}