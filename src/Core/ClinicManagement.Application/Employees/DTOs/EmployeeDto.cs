namespace ClinicManagement.Application.Employees.DTOs;

/// <summary>
/// Data Transfer Object for Employee — used in GetAll, GetById responses.
/// </summary>
public class EmployeeDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// v2.1 — IDs of doctors this employee is assigned to (Many-to-Many).
    /// </summary>
    public List<Guid> AssignedDoctorIds { get; set; } = new();

    /// <summary>
    /// Display names of assigned doctors (for UI rendering without extra calls).
    /// </summary>
    public List<string> AssignedDoctorNames { get; set; } = new();
}
