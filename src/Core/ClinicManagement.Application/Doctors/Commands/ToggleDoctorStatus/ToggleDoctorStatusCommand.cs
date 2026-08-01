using MediatR;

namespace ClinicManagement.Application.Doctors.Commands.ToggleDoctorStatus;

public class ToggleDoctorStatusCommand : IRequest
{
    public Guid Id { get; set; }
}
