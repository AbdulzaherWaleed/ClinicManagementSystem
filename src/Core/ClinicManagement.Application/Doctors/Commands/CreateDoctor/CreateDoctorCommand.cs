using MediatR;

namespace ClinicManagement.Application.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommand : IRequest<Guid>
{
    public string FullName { get; set; } = default!;
    public string? Title { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public Guid? PrimarySpecialtyId { get; set; }
}