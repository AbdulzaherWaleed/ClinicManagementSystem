using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Appointments;
using ClinicManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Appointments.Commands.ChangeAppointmentStatus;

public class ChangeAppointmentStatusCommandHandler : IRequestHandler<ChangeAppointmentStatusCommand>
{
    private readonly IApplicationDbContext _context;

    public ChangeAppointmentStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ChangeAppointmentStatusCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);

        if (appointment == null)
        {
            throw new NotFoundException(nameof(Appointment), request.AppointmentId);
        }

        // Validate logical transitions
        if (appointment.Status == AppointmentStatus.Cancelled && request.NewStatus != AppointmentStatus.Cancelled)
        {
            throw new ValidationException(new [] { new FluentValidation.Results.ValidationFailure("Status", "Cannot change status from Cancelled.") });
        }
        if (appointment.Status == AppointmentStatus.Completed && request.NewStatus != AppointmentStatus.Completed)
        {
            throw new ValidationException(new [] { new FluentValidation.Results.ValidationFailure("Status", "Cannot change status from Completed.") });
        }

        var oldStatus = appointment.Status;
        if (oldStatus == request.NewStatus)
        {
            return; // No change
        }

        appointment.Status = request.NewStatus;

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.ApplicationUserId == request.ChangedByEmployeeId, cancellationToken);

        if (employee != null)
        {
            var history = new AppointmentStatusHistory
            {
                AppointmentId = appointment.Id,
                OldStatus = oldStatus,
                NewStatus = request.NewStatus,
                ChangedByEmployeeId = employee.Id,
                ChangedAt = DateTime.UtcNow
            };
            _context.AppointmentStatusHistories.Add(history);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }
}
