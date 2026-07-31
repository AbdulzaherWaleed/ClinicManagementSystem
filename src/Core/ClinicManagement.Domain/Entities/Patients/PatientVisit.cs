using ClinicManagement.Domain.Common;
using ClinicManagement.Domain.Entities.Doctors;

namespace ClinicManagement.Domain.Entities.Patients;

public class PatientVisit : BaseAuditableEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;

    public Guid? AppointmentId { get; set; }

    public DateTime VisitDate { get; set; } = DateTime.UtcNow;
    public string? ChiefComplaint { get; set; }
    public string? Diagnosis { get; set; }
    public string? TreatmentNotes { get; set; }
    public string? Prescription { get; set; }

    public decimal? FeeCharged { get; set; }
}