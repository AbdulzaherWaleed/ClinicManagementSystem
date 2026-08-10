using ClinicManagement.Application.Reports.Queries.GetDoctorPerformance;
using ClinicManagement.Application.Reports.Queries.GetStaffPerformance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Reports_List)]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("doctor")]
    public async Task<IActionResult> GetDoctorPerformance([FromQuery] Guid doctorId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var query = new GetDoctorPerformanceQuery
        {
            DoctorId = doctorId,
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("staff")]
    public async Task<IActionResult> GetStaffPerformance([FromQuery] Guid employeeId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var query = new GetStaffPerformanceQuery
        {
            EmployeeId = employeeId,
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
