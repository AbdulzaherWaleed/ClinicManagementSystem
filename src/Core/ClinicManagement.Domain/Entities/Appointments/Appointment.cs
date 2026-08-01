using ClinicManagement.Domain.Common;
using ClinicManagement.Domain.Entities.Doctors;
using ClinicManagement.Domain.Entities.Patients;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Domain.Entities.Appointments;

public class Appointment : BaseAuditableEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;

    public Guid? CreatedByEmployeeId { get; set; }

    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Reason { get; set; }
    public string? CancellationReason { get; set; }

    // v2.1 — Visit Stage: the stage in the patient's journey (Checkup/Consultation/Procedure)
    public VisitStage VisitStage { get; set; } = VisitStage.Checkup;

    // v2.1 — Visit Type / Specialty: the service category (Botox, ENT, Ophthalmology, etc.)
    public string? VisitType { get; set; }
}