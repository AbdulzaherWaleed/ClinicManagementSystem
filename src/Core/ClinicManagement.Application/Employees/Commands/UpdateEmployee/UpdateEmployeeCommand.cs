using MediatR;

namespace ClinicManagement.Application.Employees.Commands.UpdateEmployee;

/// <summary>
/// PATCH semantics: only non-null fields are applied.
/// AssignedDoctorIds: null = no change; empty list = clear all; non-empty = full replace.
/// Excludes: Id, IsDeleted, CreatedAt, CreatedBy, ApplicationUserId (system-controlled).
/// </summary>
public class UpdateEmployeeCommand : IRequest
{
    public Guid Id { get; set; }

    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public bool? IsActive { get; set; }

    /// <summary>
    /// null  → don't touch existing assignments
    /// []    → clear all assignments
    /// [ids] → full replace with new list
    /// </summary>
    public List<Guid>? AssignedDoctorIds { get; set; }
}
