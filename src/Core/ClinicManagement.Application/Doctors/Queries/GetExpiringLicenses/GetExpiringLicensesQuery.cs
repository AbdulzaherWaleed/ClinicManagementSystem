using ClinicManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Doctors.Queries.GetExpiringLicenses;

public record ExpiringLicenseDto(Guid LicenseId, Guid DoctorId, string DoctorName, string LicenseNumber, DateTime ExpiryDate);

public record GetExpiringLicensesQuery(int DaysThreshold = 30) : IRequest<List<ExpiringLicenseDto>>;

public class GetExpiringLicensesQueryHandler : IRequestHandler<GetExpiringLicensesQuery, List<ExpiringLicenseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetExpiringLicensesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExpiringLicenseDto>> Handle(GetExpiringLicensesQuery request, CancellationToken cancellationToken)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(request.DaysThreshold);

        var expiringLicenses = await _context.DoctorLicenses
            .AsNoTracking()
            .Include(l => l.Doctor)
            .Where(l => l.IsActive && l.Doctor.IsActive && l.ExpiryDate != null && l.ExpiryDate <= thresholdDate)
            .OrderBy(l => l.ExpiryDate)
            .Select(l => new ExpiringLicenseDto(
                l.Id,
                l.DoctorId,
                l.Doctor.FullName,
                l.LicenseNumber ?? string.Empty,
                l.ExpiryDate.Value
            ))
            .ToListAsync(cancellationToken);

        return expiringLicenses;
    }
}
