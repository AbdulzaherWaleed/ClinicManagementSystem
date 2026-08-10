using ClinicManagement.Application.Appointments.DTOs;
using ClinicManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetAppointmentByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<AppointmentDto?> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(a => a.Id == request.Id && !a.IsDeleted)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient.FullName,
                PatientPhone = a.Patient.PhoneNumber,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,
                DoctorSpecialty = a.Doctor.PrimarySpecialty != null ? a.Doctor.PrimarySpecialty.Name : null,
                CreatedByEmployeeId = a.CreatedByEmployeeId,
                ScheduledStart = a.ScheduledStart,
                ScheduledEnd = a.ScheduledEnd,
                Status = a.Status.ToString(),
                Reason = a.Reason,
                CancellationReason = a.CancellationReason,
                VisitStage = a.VisitStage.ToString(),
                VisitType = a.VisitType,
                CreatedAt = a.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (appointment != null && _currentUserService.Role == "Employee" && !_currentUserService.AssignedDoctorIds.Contains(appointment.DoctorId))
        {
            throw new ClinicManagement.Application.Common.Exceptions.ForbiddenAccessException();
        }

        return appointment;
    }
}
