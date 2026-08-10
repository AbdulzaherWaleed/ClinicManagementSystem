using ClinicManagement.Application.Appointments.Queries.GetAppointmentById;
using ClinicManagement.Application.Appointments.Queries.GetAppointments;
using ClinicManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET /api/appointments — Advanced Search with optional combinable filters.
    /// Automatically enforces Employee scoping from JWT claims (Brief §2.1).
    /// Query params: patientName, phone, doctorId, visitType, visitStage, dateFrom, dateTo, status
    /// </summary>
    [HttpGet]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Bookings_List)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? patientName,
        [FromQuery] string? phone,
        [FromQuery] Guid? doctorId,
        [FromQuery] string? visitType,
        [FromQuery] string? visitStage,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        // v2.1 — Extract Employee's assigned doctor IDs from JWT claims for server-side scoping
        List<Guid>? restrictToDoctorIds = null;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userRole == "Employee")
        {
            // Collect all "doctorId" claims (multiple claims with same name — RFC 7519 best practice)
            var doctorClaims = User.FindAll("doctorId")
                .Select(c => Guid.TryParse(c.Value, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .ToList();

            restrictToDoctorIds = doctorClaims;
        }

        var query = new GetAppointmentsQuery
        {
            PatientName = patientName,
            Phone = phone,
            DoctorId = doctorId,
            VisitType = visitType,
            VisitStage = visitStage,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize,
            RestrictToDoctorIds = restrictToDoctorIds
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/appointments/{id}
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Bookings_List)]
    public async Task<IActionResult> GetById(Guid id)
    {
        // Employee scoping is enforced inside GetAppointmentByIdQueryHandler via ICurrentUserService
        var result = await _mediator.Send(new GetAppointmentByIdQuery { Id = id });
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// POST /api/appointments
    /// </summary>
    [HttpPost]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Bookings_Create)]
    public async Task<IActionResult> Create([FromBody] ClinicManagement.Application.Appointments.Commands.CreateAppointment.CreateAppointmentCommand command)
    {
        // Extract the user ID from JWT if it's an employee
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userId, out var employeeId) && User.FindFirst(ClaimTypes.Role)?.Value == "Employee")
        {
            command.CreatedByEmployeeId = employeeId;
        }

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// PATCH /api/appointments/{id} — Partial update. Only provided fields are applied.
    /// </summary>
    [HttpPatch("{id:guid}")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Bookings_Edit)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ClinicManagement.Application.Appointments.Commands.UpdateAppointment.UpdateAppointmentCommand command)
    {
        command.Id = id; // Bind ID from route to prevent client spoofing
        // Note: Employee scope check is enforced in UpdateAppointmentCommandHandler via ICurrentUserService
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// PATCH /api/appointments/{id}/cancel
    /// </summary>
    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Bookings_Edit)]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] ClinicManagement.Application.Appointments.Commands.CancelAppointment.CancelAppointmentCommand command)
    {
        if (id != command.Id) return BadRequest();
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// POST /api/appointments/{id}/status
    /// </summary>
    [HttpPost("{id:guid}/status")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Bookings_Edit)]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ClinicManagement.Domain.Enums.AppointmentStatus newStatus)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var employeeId))
        {
            return Unauthorized();
        }

        var command = new ClinicManagement.Application.Appointments.Commands.ChangeAppointmentStatus.ChangeAppointmentStatusCommand
        {
            AppointmentId = id,
            NewStatus = newStatus,
            ChangedByEmployeeId = employeeId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// GET /api/appointments/export
    /// </summary>
    [HttpGet("export")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Bookings_List)]
    public async Task<IActionResult> Export(
        [FromQuery] string? patientName,
        [FromQuery] string? phone,
        [FromQuery] Guid? doctorId,
        [FromQuery] string? visitType,
        [FromQuery] string? visitStage,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] string? status)
    {
        List<Guid>? restrictToDoctorIds = null;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userRole == "Employee")
        {
            var doctorClaims = User.FindAll("doctorId")
                .Select(c => Guid.TryParse(c.Value, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .ToList();

            restrictToDoctorIds = doctorClaims;
        }

        var query = new ClinicManagement.Application.Appointments.Queries.ExportAppointments.ExportAppointmentsQuery
        {
            PatientName = patientName,
            Phone = phone,
            DoctorId = doctorId,
            VisitType = visitType,
            VisitStage = visitStage,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Status = status,
            RestrictToDoctorIds = restrictToDoctorIds
        };

        var fileData = await _mediator.Send(query);
        return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Appointments_Export_{DateTime.UtcNow:yyyyMMddHHmm}.xlsx");
    }
}

