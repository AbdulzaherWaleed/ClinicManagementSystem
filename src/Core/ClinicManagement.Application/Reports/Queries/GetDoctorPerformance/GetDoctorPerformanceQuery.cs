using ClinicManagement.Application.Reports.DTOs;
using MediatR;

namespace ClinicManagement.Application.Reports.Queries.GetDoctorPerformance;

public class GetDoctorPerformanceQuery : IRequest<PerformanceReportDto>
{
    public Guid DoctorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
