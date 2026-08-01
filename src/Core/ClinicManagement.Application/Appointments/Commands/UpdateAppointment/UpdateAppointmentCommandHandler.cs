using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Appointments.Commands.UpdateAppointment;

public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAppointmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken);

        if (appointment == null)
            throw new NotFoundException("Appointment", request.Id);

        // Check for conflicting appointments if the time changed
        if (appointment.ScheduledStart != request.ScheduledStart || appointment.ScheduledEnd != request.ScheduledEnd)
        {
            var hasConflict = await _context.Appointments
                .AnyAsync(a => a.Id != request.Id &&
                               a.DoctorId == appointment.DoctorId &&
                               !a.IsDeleted &&
                               a.Status != AppointmentStatus.Cancelled &&
                               request.ScheduledStart < a.ScheduledEnd &&
                               request.ScheduledEnd > a.ScheduledStart,
                          cancellationToken);

            if (hasConflict)
                throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
                {
                    new("ScheduledStart", "Doctor already has an appointment during this time slot.")
                });
        }

        appointment.ScheduledStart = request.ScheduledStart;
        appointment.ScheduledEnd = request.ScheduledEnd;
        appointment.Status = request.Status;
        appointment.VisitStage = request.VisitStage;
        appointment.VisitType = request.VisitType;
        appointment.Reason = request.Reason;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
