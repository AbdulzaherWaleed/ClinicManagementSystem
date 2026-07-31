namespace ClinicManagement.Application.Doctors.DTOs;

public class DoctorDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string? Title { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? PrimarySpecialtyName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}