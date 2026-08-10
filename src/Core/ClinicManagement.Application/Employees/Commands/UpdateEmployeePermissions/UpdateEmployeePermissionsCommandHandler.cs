using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ClinicManagement.Application.Employees.Commands.UpdateEmployeePermissions;

public class UpdateEmployeePermissionsCommandHandler : IRequestHandler<UpdateEmployeePermissionsCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _context;

    public UpdateEmployeePermissionsCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService, IApplicationDbContext context)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
        _context = context;
    }

    public async Task Handle(UpdateEmployeePermissionsCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var employee = await _context.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken);
        if (employee == null)
            throw new NotFoundException(nameof(Domain.Entities.Staff.Employee), request.EmployeeId);

        if (currentUserId?.ToString() == employee.ApplicationUserId.ToString())
        {
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("Permissions", "Users cannot modify their own permissions.")
            });
        }

        var user = await _userManager.FindByIdAsync(employee.ApplicationUserId.ToString());

        if (user == null)
            throw new NotFoundException(nameof(ApplicationUser), employee.ApplicationUserId);

        var existingClaims = await _userManager.GetClaimsAsync(user);
        var permissionClaims = existingClaims.Where(c => c.Type == "Permission").ToList();

        // Remove old permission claims
        if (permissionClaims.Any())
        {
            await _userManager.RemoveClaimsAsync(user, permissionClaims);
        }

        // Add new permission claims
        if (request.Permissions.Any())
        {
            var newClaims = request.Permissions.Select(p => new Claim("Permission", p)).ToList();
            await _userManager.AddClaimsAsync(user, newClaims);
        }
    }
}
