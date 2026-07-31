namespace ClinicManagement.Application.Patients.DTOs;

public class PatientDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? NationalId { get; set; }
    public string? MedicalNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}