using ClinicManagement.Application.Patients.Commands.CreatePatient;
using ClinicManagement.Application.Patients.Commands.DeletePatient;
using ClinicManagement.Application.Patients.Queries.GetAllPatients;
using ClinicManagement.Application.Patients.Queries.GetPatientById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // كل الـ endpoints هنا محتاجة تسجيل دخول كحد أدنى
public class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        List<Guid>? restrictToDoctorIds = null;
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (userRole == "Employee")
        {
            restrictToDoctorIds = User.FindAll("doctorId")
                .Select(c => Guid.TryParse(c.Value, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .ToList();
        }

        var result = await _mediator.Send(new GetAllPatientsQuery 
        { 
            RestrictToDoctorIds = restrictToDoctorIds,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetPatientByIdQuery { Id = id });
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")] // ⭐ القاعدة بتاعتك هنا بالظبط — Admin بس
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeletePatientCommand { Id = id });
        return NoContent();
    }
}