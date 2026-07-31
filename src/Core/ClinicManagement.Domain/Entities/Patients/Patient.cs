using ClinicManagement.Domain.Common;
using ClinicManagement.Domain.Entities.Appointments;

namespace ClinicManagement.Domain.Entities.Patients;

public class Patient : BaseAuditableEntity
{
    public string FullName { get; set; } = default!;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? NationalId { get; set; }

    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }

    public string? MedicalNotes { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<PatientVisit> Visits { get; set; } = new List<PatientVisit>();
}