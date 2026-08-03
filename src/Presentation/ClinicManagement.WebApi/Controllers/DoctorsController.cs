using ClinicManagement.Application.Doctors.Commands.CreateDoctor;
using ClinicManagement.Application.Doctors.Queries.GetAllDoctors;
using ClinicManagement.Application.Doctors.Queries.GetDoctorById;
using ClinicManagement.Application.Doctors.Commands.ToggleDoctorStatus;
using ClinicManagement.Application.Doctors.Commands.DeleteDoctor;
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
    private readonly IConfiguration _configuration;

    public DoctorsController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    private bool IsDoctorLicensesEnabled()
    {
        return _configuration.GetValue<bool>("Features:DoctorLicenses", true);
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

        var result = await _mediator.Send(new GetAllDoctorsQuery 
        { 
            PageNumber = pageNumber, 
            PageSize = pageSize,
            RestrictToDoctorIds = restrictToDoctorIds
        });
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

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteDoctorCommand { Id = id });
        return NoContent();
    }

    [HttpGet("{id:guid}/licenses")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetLicenses(Guid id)
    {
        if (!IsDoctorLicensesEnabled()) return NotFound("Feature is disabled.");
        var result = await _mediator.Send(new ClinicManagement.Application.Doctors.Queries.GetDoctorLicenses.GetDoctorLicensesQuery(id));
        return Ok(result);
    }

    [HttpPost("{id:guid}/licenses")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadLicense(Guid id, [FromForm] string? licenseNumber, [FromForm] DateTime expiryDate, [FromForm] IFormFile file)
    {
        if (!IsDoctorLicensesEnabled()) return NotFound("Feature is disabled.");
        if (file == null || file.Length == 0) return BadRequest("File is required");

        using var stream = file.OpenReadStream();
        var command = new ClinicManagement.Application.Doctors.Commands.UploadDoctorLicense.UploadDoctorLicenseCommand(
            id, licenseNumber, expiryDate, file.FileName, stream);
        
        var licenseId = await _mediator.Send(command);
        return Ok(new { LicenseId = licenseId });
    }

    [HttpGet("{id:guid}/licenses/{licenseId:guid}/download")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> DownloadLicense(Guid id, Guid licenseId)
    {
        if (!IsDoctorLicensesEnabled()) return NotFound("Feature is disabled.");
        var result = await _mediator.Send(new ClinicManagement.Application.Doctors.Queries.GetDoctorLicenseFile.GetDoctorLicenseFileQuery(id, licenseId));
        return File(result.FileStream, result.ContentType, result.FileName);
    }

    [HttpGet("licenses/expiring")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<List<ClinicManagement.Application.Doctors.Queries.GetExpiringLicenses.ExpiringLicenseDto>>> GetExpiringLicenses([FromQuery] int days = 30)
    {
        if (!IsDoctorLicensesEnabled()) return NotFound("Feature is disabled.");
        var result = await _mediator.Send(new ClinicManagement.Application.Doctors.Queries.GetExpiringLicenses.GetExpiringLicensesQuery(days));
        return Ok(result);
    }
}