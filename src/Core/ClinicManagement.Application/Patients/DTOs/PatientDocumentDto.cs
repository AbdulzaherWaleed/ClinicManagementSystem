namespace ClinicManagement.Application.Patients.DTOs;

public class PatientDocumentDto
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
