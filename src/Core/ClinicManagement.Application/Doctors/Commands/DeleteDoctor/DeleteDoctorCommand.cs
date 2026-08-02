using MediatR;

namespace ClinicManagement.Application.Doctors.Commands.DeleteDoctor;

public class DeleteDoctorCommand : IRequest
{
    public Guid Id { get; set; }
}
