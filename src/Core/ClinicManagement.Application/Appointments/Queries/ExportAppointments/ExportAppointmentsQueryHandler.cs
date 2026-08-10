using ClinicManagement.Application.Appointments.DTOs;
using ClinicManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Appointments.Queries.ExportAppointments;

public class ExportAppointmentsQueryHandler : IRequestHandler<ExportAppointmentsQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IExcelService _excelService;

    public ExportAppointmentsQueryHandler(IApplicationDbContext context, IExcelService excelService)
    {
        _context = context;
        _excelService = excelService;
    }

    public async Task<byte[]> Handle(ExportAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Appointments
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(a => !a.IsDeleted)
            .AsQueryable();

        // Apply all optional filters (AND logic)
        if (!string.IsNullOrWhiteSpace(request.PatientName))
            query = query.Where(a => a.Patient.FullName.Contains(request.PatientName));

        if (!string.IsNullOrWhiteSpace(request.Phone))
            query = query.Where(a => a.Patient.PhoneNumber != null &&
                                     a.Patient.PhoneNumber.Contains(request.Phone));

        if (request.DoctorId.HasValue)
            query = query.Where(a => a.DoctorId == request.DoctorId.Value);

        if (!string.IsNullOrWhiteSpace(request.VisitType))
            query = query.Where(a => a.VisitType != null && a.VisitType.Contains(request.VisitType));

        if (!string.IsNullOrWhiteSpace(request.VisitStage) &&
            Enum.TryParse<ClinicManagement.Domain.Enums.VisitStage>(request.VisitStage, out var visitStage))
            query = query.Where(a => a.VisitStage == visitStage);

        if (request.DateFrom.HasValue)
            query = query.Where(a => a.ScheduledStart >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            query = query.Where(a => a.ScheduledStart <= request.DateTo.Value.AddDays(1).AddTicks(-1));

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<ClinicManagement.Domain.Enums.AppointmentStatus>(request.Status, out var status))
            query = query.Where(a => a.Status == status);

        if (request.RestrictToDoctorIds != null)
            query = query.Where(a => request.RestrictToDoctorIds.Contains(a.DoctorId));

        var appointments = await query
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderByDescending(a => a.ScheduledStart)
            .ToListAsync(cancellationToken);

        var dtos = appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            PatientId = a.PatientId,
            PatientName = a.Patient?.FullName,
            PatientPhone = a.Patient?.PhoneNumber,
            DoctorId = a.DoctorId,
            DoctorName = a.Doctor?.FullName,
            ScheduledStart = a.ScheduledStart,
            ScheduledEnd = a.ScheduledEnd,
            Status = a.Status.ToString(),
            Reason = a.Reason,
            VisitStage = a.VisitStage.ToString(),
            VisitType = a.VisitType
        }).ToList();

        return _excelService.ExportAppointmentsToExcel(dtos);
    }
}
