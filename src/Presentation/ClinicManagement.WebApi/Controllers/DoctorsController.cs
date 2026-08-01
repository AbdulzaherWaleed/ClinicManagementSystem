using ClinicManagement.Application.Doctors.Commands.CreateDoctor;
using ClinicManagement.Application.Doctors.Queries.GetAllDoctors;
using ClinicManagement.Application.Doctors.Queries.GetDoctorById;
using ClinicManagement.Application.Doctors.Commands.ToggleDoctorStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Standard users can view, but only Admin can mutate
public class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DoctorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllDoctorsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetDoctorByIdQuery { Id = id });
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateDoctorCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPost("{id:guid}/toggle-status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        await _mediator.Send(new ToggleDoctorStatusCommand { Id = id });
        return NoContent();
    }
}