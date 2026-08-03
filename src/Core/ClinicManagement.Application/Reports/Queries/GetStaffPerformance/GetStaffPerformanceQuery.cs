using ClinicManagement.Application.Reports.DTOs;
using MediatR;

namespace ClinicManagement.Application.Reports.Queries.GetStaffPerformance;

public class GetStaffPerformanceQuery : IRequest<PerformanceReportDto>
{
    public Guid EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
