using ClinicManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ClinicManagement.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = default!;
    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    // v2.1 — Many-to-Many: Employee can be assigned to multiple doctors.
    // Stored as a comma-separated string in DB (simple, no extra join table on Identity side).
    // Parsed to List<Guid> by the JWT generator and Login handler.
    public string? AssignedDoctorIdsRaw { get; set; }

    /// <summary>
    /// Convenience property — NOT mapped to DB. Parsed from AssignedDoctorIdsRaw.
    /// </summary>
    public List<Guid> AssignedDoctorIds =>
        string.IsNullOrWhiteSpace(AssignedDoctorIdsRaw)
            ? new List<Guid>()
            : AssignedDoctorIdsRaw
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Guid.TryParse(s.Trim(), out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .ToList();

    public DateTime? LastLoginAt { get; set; }

    public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
}