namespace ClinicManagement.Application.Auth.DTOs;

public class AuthResponseDto
{
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = default!;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;

    /// <summary>
    /// v2.1 — Many-to-Many: list of doctor IDs this employee is assigned to.
    /// Empty for Admin and Doctor roles.
    /// Used by the frontend to enforce Employee scoping (§2.1 of the brief).
    /// </summary>
    public List<Guid> AssignedDoctorIds { get; set; } = new();
}