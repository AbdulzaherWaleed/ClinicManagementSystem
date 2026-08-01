using ClinicManagement.Domain.Common;
using ClinicManagement.Domain.Entities.Appointments;
using ClinicManagement.Domain.Entities.Patients;
using ClinicManagement.Domain.Entities.Settings;
using ClinicManagement.Domain.Entities.Staff;

namespace ClinicManagement.Domain.Entities.Doctors;

public class Doctor : BaseAuditableEntity
{
    public string FullName { get; set; } = default!;
    public string? Title { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public Guid? PrimarySpecialtyId { get; set; }
    public DoctorSpecialty? PrimarySpecialty { get; set; }

    public Guid? ApplicationUserId { get; set; }

    public bool IsActive { get; set; } = true;

    // Many-to-Many: one doctor can have multiple employees assigned
    public ICollection<EmployeeDoctorAssignment> EmployeeAssignments { get; set; } = new List<EmployeeDoctorAssignment>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<PatientVisit> PatientVisits { get; set; } = new List<PatientVisit>();
    public ICollection<DoctorDocument> Documents { get; set; } = new List<DoctorDocument>();
    public DoctorCustomization? Customization { get; set; }
}