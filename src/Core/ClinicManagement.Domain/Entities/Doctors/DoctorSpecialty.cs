using ClinicManagement.Domain.Common;

namespace ClinicManagement.Domain.Entities.Doctors;

public class DoctorSpecialty : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}