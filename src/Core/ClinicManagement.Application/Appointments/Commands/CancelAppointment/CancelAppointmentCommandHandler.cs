using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand>
{
    private readonly IApplicationDbContext _context;

    public CancelAppointmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken);

        if (appointment == null)
            throw new NotFoundException("Appointment", request.Id);

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.CancellationReason = request.CancellationReason;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
