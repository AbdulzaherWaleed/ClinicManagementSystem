using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Application.Appointments.DTOs;

/// <summary>
/// Full appointment data including patient journey information.
/// </summary>
public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = default!;
    public string? PatientPhone { get; set; }

    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = default!;
    public string? DoctorSpecialty { get; set; }

    public Guid? CreatedByEmployeeId { get; set; }

    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }

    public string Status { get; set; } = default!;
    public string? Reason { get; set; }
    public string? CancellationReason { get; set; }

    // v2.1 — Visit journey fields
    public string VisitStage { get; set; } = "Checkup"; // Checkup | Consultation | Procedure
    public string? VisitType { get; set; }              // Botox | ENT | Ophthalmology | etc.

    public DateTime CreatedAt { get; set; }

    public List<AppointmentStatusHistoryDto> StatusHistories { get; set; } = new();
}

