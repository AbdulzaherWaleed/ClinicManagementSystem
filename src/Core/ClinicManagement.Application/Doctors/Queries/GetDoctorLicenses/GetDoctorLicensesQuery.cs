using ClinicManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ClinicManagement.Application.Doctors.Queries.GetDoctorLicenses;

public record DoctorLicenseDto(Guid Id, Guid DoctorId, string? LicenseNumber, string FilePath, string OriginalFileName, string FileType, DateTime? ExpiryDate, bool IsActive);

public record GetDoctorLicensesQuery(Guid DoctorId) : IRequest<List<DoctorLicenseDto>>;

public class GetDoctorLicensesQueryHandler : IRequestHandler<GetDoctorLicensesQuery, List<DoctorLicenseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDoctorLicensesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DoctorLicenseDto>> Handle(GetDoctorLicensesQuery request, CancellationToken cancellationToken)
    {
        var licenses = await _context.DoctorLicenses
            .AsNoTracking()
            .Where(x => x.DoctorId == request.DoctorId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new DoctorLicenseDto(
                x.Id,
                x.DoctorId,
                x.LicenseNumber,
                x.FilePath,
                x.OriginalFileName,
                x.FileType,
                x.ExpiryDate,
                x.IsActive
            ))
            .ToListAsync(cancellationToken);

        return licenses;
    }
}
