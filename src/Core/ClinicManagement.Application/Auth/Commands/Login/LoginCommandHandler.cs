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

    public LoginCommandHandler(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.IsActive)
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("Email", "Invalid email or password.")
            });

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValid)
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("Password", "Invalid email or password.")
            });

        var roles = await _userManager.GetRolesAsync(user);

        // v2.1 — Generate JWT with multiple "doctorId" claims (one per assigned doctor)
        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

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