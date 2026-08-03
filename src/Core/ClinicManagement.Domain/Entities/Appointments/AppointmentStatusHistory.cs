using ClinicManagement.Domain.Common;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Entities.Staff;

namespace ClinicManagement.Domain.Entities.Appointments;

public class AppointmentStatusHistory : BaseAuditableEntity
{
    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = default!;

    public AppointmentStatus OldStatus { get; set; }
    public AppointmentStatus NewStatus { get; set; }
    
    public Guid ChangedByEmployeeId { get; set; }
    public Employee ChangedByEmployee { get; set; } = default!;

    public DateTime ChangedAt { get; set; }
}
