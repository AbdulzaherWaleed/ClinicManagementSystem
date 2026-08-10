using ClinicManagement.Application.Employees.Commands.CreateEmployee;
using ClinicManagement.Application.Employees.Commands.DeleteEmployee;
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
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Employees_List)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllEmployeesQuery { PageNumber = pageNumber, PageSize = pageSize });
        return Ok(result);
    }

    /// <summary>
    /// GET /api/employees/{id} — Admin only. Returns employee with doctor assignments.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Employees_List)]
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
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Employees_Create)]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// POST /api/employees/register — Admin only. Registers an employee with identity account.
    /// </summary>
    [HttpPost("register")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Employees_Create)]
    public async Task<IActionResult> Register([FromBody] RegisterEmployeeCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// PATCH /api/employees/{id} — Admin only. Partial update of employee fields and/or doctor assignments.
    /// AssignedDoctorIds: null = no change, [] = clear all, [ids] = full replace.
    /// </summary>
    [HttpPatch("{id:guid}")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Employees_Edit)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ClinicManagement.Application.Employees.Commands.UpdateEmployee.UpdateEmployeeCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// POST /api/employees/{id}/toggle-status — Admin only. Toggles active status.
    /// </summary>
    [HttpPost("{id:guid}/toggle-status")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Employees_Edit)]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        await _mediator.Send(new ToggleEmployeeStatusCommand { Id = id });
        return NoContent();
    }

    /// <summary>
    /// DELETE /api/employees/{id} — Admin only. Soft deletes employee.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Employees_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteEmployeeCommand { Id = id });
        return NoContent();
    }

    /// <summary>
    /// GET /api/employees/{id}/permissions
    /// </summary>
    [HttpGet("{id:guid}/permissions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPermissions(Guid id)
    {
        var result = await _mediator.Send(new ClinicManagement.Application.Employees.Queries.GetEmployeePermissions.GetEmployeePermissionsQuery { EmployeeId = id });
        return Ok(result);
    }

    /// <summary>
    /// PUT /api/employees/{id}/permissions
    /// </summary>
    [HttpPut("{id:guid}/permissions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePermissions(Guid id, [FromBody] ClinicManagement.Application.Employees.Commands.UpdateEmployeePermissions.UpdateEmployeePermissionsCommand command)
    {
        command.EmployeeId = id;
        await _mediator.Send(command);
        return NoContent();
    }
}
