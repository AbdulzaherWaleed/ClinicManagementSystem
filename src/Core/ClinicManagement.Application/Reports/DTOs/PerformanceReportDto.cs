namespace ClinicManagement.Application.Reports.DTOs;

public class PerformanceReportDto
{
    public int TotalBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int CancelledOrNoShowBookings { get; set; }
    public double CompletionRate { get; set; }
    public Dictionary<string, int> BookingsByVisitStage { get; set; } = new();
    public Dictionary<string, int> BookingsByDoctor { get; set; } = new();
}
