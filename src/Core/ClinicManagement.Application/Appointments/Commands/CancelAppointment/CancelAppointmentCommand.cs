using MediatR;

namespace ClinicManagement.Application.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommand : IRequest
{
    public Guid Id { get; set; }
    public string? CancellationReason { get; set; }
}
