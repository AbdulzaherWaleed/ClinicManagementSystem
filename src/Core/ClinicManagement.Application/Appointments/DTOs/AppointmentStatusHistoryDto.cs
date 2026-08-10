namespace ClinicManagement.Application.Appointments.DTOs;

public class AppointmentStatusHistoryDto
{
    public Guid Id { get; set; }
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string? ChangedByEmployeeName { get; set; }
}
