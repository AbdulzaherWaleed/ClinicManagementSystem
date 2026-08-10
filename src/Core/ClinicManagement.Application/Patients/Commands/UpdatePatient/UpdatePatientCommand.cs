using MediatR;

namespace ClinicManagement.Application.Patients.Commands.UpdatePatient;

/// <summary>
/// PATCH semantics: only non-null fields are applied. Null = "don't touch".
/// Excludes: Id, IsDeleted, CreatedAt, CreatedBy, LastModifiedBy (all system-controlled).
/// </summary>
public class UpdatePatientCommand : IRequest
{
    public Guid Id { get; set; }

    public string? FullName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? NationalId { get; set; }
    public string? MedicalNotes { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
}
