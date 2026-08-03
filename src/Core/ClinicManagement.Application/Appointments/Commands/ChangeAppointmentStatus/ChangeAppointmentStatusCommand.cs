using ClinicManagement.Domain.Enums;
using MediatR;

namespace ClinicManagement.Application.Appointments.Commands.ChangeAppointmentStatus;

public class ChangeAppointmentStatusCommand : IRequest
{
    public Guid AppointmentId { get; set; }
    public AppointmentStatus NewStatus { get; set; }
    public Guid ChangedByEmployeeId { get; set; }
}
