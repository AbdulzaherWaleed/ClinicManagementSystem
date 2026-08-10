using ClinicManagement.Application.Patients.Commands.CreatePatient;
using ClinicManagement.Application.Patients.Commands.DeletePatient;
using ClinicManagement.Application.Patients.Commands.UpdatePatient;
using ClinicManagement.Application.Patients.Queries.GetAllPatients;
using ClinicManagement.Application.Patients.Queries.GetPatientById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Patients_List)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        List<Guid>? restrictToDoctorIds = null;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userRole == "Employee")
        {
            restrictToDoctorIds = User.FindAll("doctorId")
                .Select(c => Guid.TryParse(c.Value, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .ToList();
        }
        else if (userRole == "Doctor")
        {
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var doctorId))
            {
                restrictToDoctorIds = new List<Guid> { doctorId };
            }
        }

        var result = await _mediator.Send(new GetAllPatientsQuery
        {
            RestrictToDoctorIds = restrictToDoctorIds,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm
        });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Patients_List)]
    public async Task<IActionResult> GetById(Guid id)
    {
        List<Guid>? restrictToDoctorIds = null;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userRole == "Employee")
        {
            restrictToDoctorIds = User.FindAll("doctorId")
                .Select(c => Guid.TryParse(c.Value, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .ToList();
        }
        else if (userRole == "Doctor")
        {
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var doctorId))
            {
                restrictToDoctorIds = new List<Guid> { doctorId };
            }
        }

        var result = await _mediator.Send(new GetPatientByIdQuery
        {
            Id = id,
            RestrictToDoctorIds = restrictToDoctorIds
        });
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Patients_Create)]
    public async Task<IActionResult> Create([FromBody] CreatePatientCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// PATCH /api/patients/{id} — Partial update. Only provided fields are applied.
    /// Employees are restricted to patients within their assigned doctor scope.
    /// </summary>
    [HttpPatch("{id:guid}")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Patients_Edit)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Patients_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeletePatientCommand { Id = id });
        return NoContent();
    }

    // --- Patient Profile & Documents ---

    [HttpGet("{id:guid}/profile")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Patients_List)]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        List<Guid>? restrictToDoctorIds = null;
        if (User.FindFirst(ClaimTypes.Role)?.Value == "Employee")
        {
            restrictToDoctorIds = User.FindAll("doctorId")
                .Select(c => Guid.TryParse(c.Value, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty).ToList();
        }
        else if (User.FindFirst(ClaimTypes.Role)?.Value == "Doctor")
        {
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var doctorId))
            {
                restrictToDoctorIds = new List<Guid> { doctorId };
            }
        }

        var result = await _mediator.Send(new ClinicManagement.Application.Patients.Queries.GetPatientProfile.GetPatientProfileQuery
        {
            Id = id,
            RestrictToDoctorIds = restrictToDoctorIds
        });
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("{id:guid}/appointments")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Patients_List)]
    public async Task<IActionResult> GetPatientAppointments(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        List<Guid>? restrictToDoctorIds = null;
        if (User.FindFirst(ClaimTypes.Role)?.Value == "Employee")
        {
            restrictToDoctorIds = User.FindAll("doctorId")
                .Select(c => Guid.TryParse(c.Value, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty).ToList();
        }
        else if (User.FindFirst(ClaimTypes.Role)?.Value == "Doctor")
        {
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var doctorId))
            {
                restrictToDoctorIds = new List<Guid> { doctorId };
            }
        }

        var result = await _mediator.Send(new ClinicManagement.Application.Patients.Queries.GetPatientAppointments.GetPatientAppointmentsQuery
        {
            PatientId = id,
            RestrictToDoctorIds = restrictToDoctorIds,
            PageNumber = page,
            PageSize = pageSize
        });
        return Ok(result);
    }

    [HttpGet("{id:guid}/documents")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Patients_List)]
    public async Task<IActionResult> GetPatientDocuments(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        List<Guid>? restrictToDoctorIds = null;
        if (User.FindFirst(ClaimTypes.Role)?.Value == "Employee")
        {
            restrictToDoctorIds = User.FindAll("doctorId")
                .Select(c => Guid.TryParse(c.Value, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty).ToList();
        }
        else if (User.FindFirst(ClaimTypes.Role)?.Value == "Doctor")
        {
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var doctorId))
            {
                restrictToDoctorIds = new List<Guid> { doctorId };
            }
        }

        var result = await _mediator.Send(new ClinicManagement.Application.Patients.Queries.GetPatientDocuments.GetPatientDocumentsQuery
        {
            PatientId = id,
            RestrictToDoctorIds = restrictToDoctorIds,
            PageNumber = page,
            PageSize = pageSize
        });
        return Ok(result);
    }

    [HttpPost("{id:guid}/documents")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Patients_Edit)] // Using Edit for uploading docs
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
    public async Task<IActionResult> UploadDocument(
        Guid id, 
        [FromForm] string documentType, 
        [FromForm] string? notes, 
        IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("File is required.");

        List<Guid>? restrictToDoctorIds = null;
        if (User.FindFirst(ClaimTypes.Role)?.Value == "Employee")
        {
            restrictToDoctorIds = User.FindAll("doctorId")
                .Select(c => Guid.TryParse(c.Value, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty).ToList();
        }
        else if (User.FindFirst(ClaimTypes.Role)?.Value == "Doctor")
        {
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var doctorId))
            {
                restrictToDoctorIds = new List<Guid> { doctorId };
            }
        }

        using var stream = file.OpenReadStream();
        var documentId = await _mediator.Send(new ClinicManagement.Application.Patients.Commands.UploadPatientDocument.UploadPatientDocumentCommand
        {
            PatientId = id,
            DocumentType = documentType,
            Notes = notes,
            FileName = file.FileName,
            FileStream = stream,
            RestrictToDoctorIds = restrictToDoctorIds
        });

        return Ok(new { id = documentId });
    }

    [HttpGet("{id:guid}/documents/{documentId:guid}/download")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Patients_List)]
    public async Task<IActionResult> DownloadDocument(Guid id, Guid documentId)
    {
        List<Guid>? restrictToDoctorIds = null;
        if (User.FindFirst(ClaimTypes.Role)?.Value == "Employee")
        {
            restrictToDoctorIds = User.FindAll("doctorId")
                .Select(c => Guid.TryParse(c.Value, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty).ToList();
        }
        else if (User.FindFirst(ClaimTypes.Role)?.Value == "Doctor")
        {
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var doctorId))
            {
                restrictToDoctorIds = new List<Guid> { doctorId };
            }
        }

        var result = await _mediator.Send(new ClinicManagement.Application.Patients.Queries.DownloadPatientDocument.DownloadPatientDocumentQuery
        {
            PatientId = id,
            DocumentId = documentId,
            RestrictToDoctorIds = restrictToDoctorIds
        });

        return File(result.FileStream, result.ContentType, result.FileName);
    }

    [HttpDelete("{id:guid}/documents/{documentId:guid}")]
    [Authorize(Policy = ClinicManagement.Domain.Constants.AppPermissions.Patients_Edit)] // Edit permission because we are editing the patient's record
    public async Task<IActionResult> DeleteDocument(Guid id, Guid documentId)
    {
        List<Guid>? restrictToDoctorIds = null;
        if (User.FindFirst(ClaimTypes.Role)?.Value == "Employee")
        {
            restrictToDoctorIds = User.FindAll("doctorId")
                .Select(c => Guid.TryParse(c.Value, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty).ToList();
        }
        else if (User.FindFirst(ClaimTypes.Role)?.Value == "Doctor")
        {
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var doctorId))
            {
                restrictToDoctorIds = new List<Guid> { doctorId };
            }
        }

        await _mediator.Send(new ClinicManagement.Application.Patients.Commands.DeletePatientDocument.DeletePatientDocumentCommand
        {
            PatientId = id,
            DocumentId = documentId,
            RestrictToDoctorIds = restrictToDoctorIds
        });
        return NoContent();
    }
}