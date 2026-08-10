using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Application.Patients.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDto?>
{
    private readonly IApplicationDbContext _context;

    public GetPatientByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PatientDto?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        // IDOR check: if scoping is active, patient must have an appointment with an assigned doctor
        if (request.RestrictToDoctorIds is { Count: > 0 })
        {
            var hasAccess = await _context.Appointments
                .AnyAsync(a => a.PatientId == request.Id
                            && request.RestrictToDoctorIds.Contains(a.DoctorId),
                          cancellationToken);

            if (!hasAccess)
                throw new ForbiddenAccessException();
        }

        return await _context.Patients
            .AsNoTracking()
            .Where(p => p.Id == request.Id)
            .Select(p => new PatientDto
            {
                Id             = p.Id,
                FullName       = p.FullName,
                DateOfBirth    = p.DateOfBirth,
                Gender         = p.Gender,
                PhoneNumber    = p.PhoneNumber,
                Email          = p.Email,
                Address        = p.Address,
                NationalId     = p.NationalId,
                MedicalNotes   = p.MedicalNotes,
                CreatedAt      = p.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}