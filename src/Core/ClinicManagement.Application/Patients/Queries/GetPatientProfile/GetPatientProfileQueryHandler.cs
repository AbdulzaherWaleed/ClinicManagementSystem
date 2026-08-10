using ClinicManagement.Application.Appointments.DTOs;
using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Application.Patients.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Patients.Queries.GetPatientProfile;

public class GetPatientProfileQueryHandler : IRequestHandler<GetPatientProfileQuery, PatientProfileDto?>
{
    private readonly IApplicationDbContext _context;

    public GetPatientProfileQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PatientProfileDto?> Handle(GetPatientProfileQuery request, CancellationToken cancellationToken)
    {
        // 1. IDOR Check: Ensure the employee has access to this patient
        if (request.RestrictToDoctorIds is { Count: > 0 })
        {
            var hasAccess = await _context.Appointments
                .AnyAsync(a => a.PatientId == request.Id
                            && request.RestrictToDoctorIds.Contains(a.DoctorId),
                          cancellationToken);

            if (!hasAccess)
                throw new ForbiddenAccessException();
        }

        // 2. Fetch the patient details
        var patient = await _context.Patients
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

        if (patient is null) return null;

        // 3. Calculate summary directly from DB without loading all records
        var now = DateTime.UtcNow;

        var totalVisits = await _context.Appointments
            .CountAsync(a => a.PatientId == request.Id && !a.IsDeleted && a.Status == ClinicManagement.Domain.Enums.AppointmentStatus.Completed, cancellationToken);

        var lastVisit = await _context.Appointments
            .Where(a => a.PatientId == request.Id && !a.IsDeleted && a.Status == ClinicManagement.Domain.Enums.AppointmentStatus.Completed)
            .OrderByDescending(a => a.ScheduledStart)
            .Select(a => a.ScheduledStart)
            .FirstOrDefaultAsync(cancellationToken);

        var nextAppointment = await _context.Appointments
            .Where(a => a.PatientId == request.Id && !a.IsDeleted && a.Status != ClinicManagement.Domain.Enums.AppointmentStatus.Cancelled && a.ScheduledStart > now)
            .OrderBy(a => a.ScheduledStart)
            .Select(a => a.ScheduledStart)
            .FirstOrDefaultAsync(cancellationToken);

        // 4. Build final profile
        return new PatientProfileDto
        {
            Demographics = patient,
            TotalVisits = totalVisits,
            LastVisitDate = lastVisit != default ? lastVisit : null,
            NextAppointmentDate = nextAppointment != default ? nextAppointment : null
        };
    }
}
