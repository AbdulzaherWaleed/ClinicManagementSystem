using ClinicManagement.Domain.Entities.Doctors;
using ClinicManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Numerics;

namespace ClinicManagement.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = default!;
    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? AssignedDoctorId { get; set; }
    public Doctor? AssignedDoctor { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
}