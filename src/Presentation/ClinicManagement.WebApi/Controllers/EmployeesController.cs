using ClinicManagement.Application.Employees.Commands.CreateEmployee;
using ClinicManagement.Application.Employees.Commands.RegisterEmployee;
using ClinicManagement.Application.Employees.Commands.ToggleEmployeeStatus;
using ClinicManagement.Application.Employees.Queries.GetAllEmployees;
using ClinicManagement.Application.Employees.Queries.GetEmployeeById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET /api/employees — Admin only. Returns all employees with their doctor assignments.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllEmployeesQuery());
        return Ok(result);
    }

    /// <summary>
    /// GET /api/employees/{id} — Admin only. Returns employee with doctor assignments.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetEmployeeByIdQuery { Id = id });
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// POST /api/employees — Admin only. Creates employee with M2M doctor assignments.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// POST /api/employees/register — Admin only. Registers an employee with identity account.
    /// </summary>
    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterEmployeeCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// POST /api/employees/{id}/toggle-status — Admin only. Toggles active status.
    /// </summary>
    [HttpPost("{id:guid}/toggle-status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        await _mediator.Send(new ToggleEmployeeStatusCommand { Id = id });
        return NoContent();
    }
}
