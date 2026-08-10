using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Appointments.Commands.UpdateAppointment;

public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAppointmentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken);

        if (appointment == null)
            throw new NotFoundException("Appointment", request.Id);

        // Row-level security: Employee may only edit appointments for their assigned doctors
        if (_currentUserService.Role == "Employee" && !_currentUserService.AssignedDoctorIds.Contains(appointment.DoctorId))
            throw new ForbiddenAccessException();

        // Resolve effective values (new value ?? existing value) for conflict check
        var newStart = request.ScheduledStart ?? appointment.ScheduledStart;
        var newEnd   = request.ScheduledEnd   ?? appointment.ScheduledEnd;

        // Check for conflicting appointments only if the time actually changed
        bool timeChanged = request.ScheduledStart.HasValue || request.ScheduledEnd.HasValue;
        if (timeChanged)
        {
            var hasConflict = await _context.Appointments
                .AnyAsync(a => a.Id != request.Id &&
                               a.DoctorId == appointment.DoctorId &&
                               !a.IsDeleted &&
                               a.Status != AppointmentStatus.Cancelled &&
                               newStart < a.ScheduledEnd &&
                               newEnd > a.ScheduledStart,
                          cancellationToken);

            if (hasConflict)
                throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
                {
                    new("ScheduledStart", "Doctor already has an appointment during this time slot.")
                });
        }

        // Apply only the fields that were explicitly provided (PATCH semantics)
        if (request.ScheduledStart.HasValue) appointment.ScheduledStart = request.ScheduledStart.Value;
        if (request.ScheduledEnd.HasValue)   appointment.ScheduledEnd   = request.ScheduledEnd.Value;
        if (request.Status.HasValue)         appointment.Status         = request.Status.Value;
        if (request.VisitStage.HasValue)     appointment.VisitStage     = request.VisitStage.Value;
        if (request.VisitType != null)       appointment.VisitType      = request.VisitType;
        if (request.Reason != null)          appointment.Reason         = request.Reason;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
