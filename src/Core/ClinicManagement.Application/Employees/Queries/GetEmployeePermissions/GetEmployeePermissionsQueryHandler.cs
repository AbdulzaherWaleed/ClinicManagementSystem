using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

using ClinicManagement.Application.Common.Interfaces;

namespace ClinicManagement.Application.Employees.Queries.GetEmployeePermissions;

public class GetEmployeePermissionsQueryHandler : IRequestHandler<GetEmployeePermissionsQuery, List<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public GetEmployeePermissionsQueryHandler(UserManager<ApplicationUser> userManager, IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<List<string>> Handle(GetEmployeePermissionsQuery request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken);
        if (employee == null)
            throw new NotFoundException(nameof(Domain.Entities.Staff.Employee), request.EmployeeId);

        var user = await _userManager.FindByIdAsync(employee.ApplicationUserId.ToString());

        if (user == null)
            throw new NotFoundException(nameof(ApplicationUser), employee.ApplicationUserId);

        var claims = await _userManager.GetClaimsAsync(user);

        return claims
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .ToList();
    }
}
