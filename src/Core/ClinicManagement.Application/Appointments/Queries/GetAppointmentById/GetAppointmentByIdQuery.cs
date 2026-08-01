using ClinicManagement.Application.Appointments.DTOs;
using MediatR;

namespace ClinicManagement.Application.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQuery : IRequest<AppointmentDto?>
{
    public Guid Id { get; set; }
}
