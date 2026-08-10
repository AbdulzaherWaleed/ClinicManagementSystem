using ClinicManagement.Application.Appointments.DTOs;
using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Patients.Queries.GetPatientAppointments;

public class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, PaginatedList<AppointmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPatientAppointmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<AppointmentDto>> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
    {
        // 1. IDOR Check
        if (request.RestrictToDoctorIds is { Count: > 0 })
        {
            var hasAccess = await _context.Appointments
                .AnyAsync(a => a.PatientId == request.PatientId
                            && request.RestrictToDoctorIds.Contains(a.DoctorId),
                          cancellationToken);

            if (!hasAccess)
                throw new ForbiddenAccessException();
        }

        // 2. Query with Select projection directly to DTO
        var query = _context.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == request.PatientId && !a.IsDeleted)
            .OrderByDescending(a => a.ScheduledStart)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                DoctorName = a.Doctor.FullName,
                DoctorSpecialty = a.Doctor.PrimarySpecialty != null ? a.Doctor.PrimarySpecialty.Name : null,
                ScheduledStart = a.ScheduledStart,
                ScheduledEnd = a.ScheduledEnd,
                Status = a.Status.ToString(),
                VisitType = a.VisitType,
                VisitStage = a.VisitStage.ToString(),
                Reason = a.Reason,
                StatusHistories = a.StatusHistories.OrderByDescending(sh => sh.ChangedAt).Select(sh => new AppointmentStatusHistoryDto
                {
                    Id = sh.Id,
                    OldStatus = sh.OldStatus.ToString(),
                    NewStatus = sh.NewStatus.ToString(),
                    ChangedAt = sh.ChangedAt,
                    ChangedByEmployeeName = sh.ChangedByEmployee != null ? sh.ChangedByEmployee.FullName : null
                }).ToList()
            });

        // 3. Paginate
        return await PaginatedList<AppointmentDto>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);
    }
}
