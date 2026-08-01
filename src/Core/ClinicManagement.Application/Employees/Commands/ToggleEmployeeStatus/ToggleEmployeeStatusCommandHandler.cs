using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClinicManagement.Application.Employees.Commands.ToggleEmployeeStatus;

public class ToggleEmployeeStatusCommandHandler : IRequestHandler<ToggleEmployeeStatusCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ToggleEmployeeStatusCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task Handle(ToggleEmployeeStatusCommand request, CancellationToken cancellationToken)
    {
        // 1. Find the employee
        var employee = await _context.Employees.FindAsync(new object[] { request.Id }, cancellationToken);

        if (employee == null)
            throw new NotFoundException(nameof(employee), request.Id);

        // 2. Toggle status in domain entity
        employee.IsActive = !employee.IsActive;

        // 3. Toggle status in identity user
        if (employee.ApplicationUserId != Guid.Empty)
        {
            var user = await _userManager.FindByIdAsync(employee.ApplicationUserId.ToString());
            if (user != null)
            {
                user.IsActive = employee.IsActive;
                await _userManager.UpdateAsync(user);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
