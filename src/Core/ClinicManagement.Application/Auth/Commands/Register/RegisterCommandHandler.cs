using ClinicManagement.Application.Auth.DTOs;
using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClinicManagement.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("Email", "This email is already registered.")
            });

        var roleNameValue = request.Role.ToString();

        if (!await _roleManager.RoleExistsAsync(roleNameValue))
            await _roleManager.CreateAsync(new IdentityRole<Guid>(roleNameValue));

        // v2.1 — Store multiple doctor IDs as comma-separated in AssignedDoctorIdsRaw
        var assignedDoctorIdsRaw = request.AssignedDoctorIds.Count > 0
            ? string.Join(",", request.AssignedDoctorIds)
            : null;

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Role = request.Role,
            AssignedDoctorIdsRaw = assignedDoctorIdsRaw,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new ValidationException(result.Errors.Select(e =>
                new FluentValidation.Results.ValidationFailure(string.Empty, e.Description)));

        await _userManager.AddToRoleAsync(user, roleNameValue);

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user, new List<string> { roleNameValue }, new List<System.Security.Claims.Claim>());
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
                Role = roleNameValue,
                AssignedDoctorIds = user.AssignedDoctorIds // parsed from raw string
            }
        };
    }
}