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
        var appointments = await _context.Appointments
            .AsNoTracking()
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
