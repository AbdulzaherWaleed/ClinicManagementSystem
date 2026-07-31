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
    public Guid? AssignedDoctorId { get; set; }
}