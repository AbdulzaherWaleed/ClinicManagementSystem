using MediatR;

namespace ClinicManagement.Application.Doctors.Commands.UpdateDoctor;

/// <summary>
/// PATCH semantics: only non-null fields are applied. Null = "don't touch".
/// Excludes all system-controlled fields (Id, IsDeleted, CreatedAt, CreatedBy, etc.).
/// </summary>
public class UpdateDoctorCommand : IRequest
{
    public Guid Id { get; set; }

    public string? FullName { get; set; }
    public string? Title { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public Guid? PrimarySpecialtyId { get; set; }
    public bool? IsActive { get; set; }
}
