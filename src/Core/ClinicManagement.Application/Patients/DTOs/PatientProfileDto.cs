using ClinicManagement.Application.Appointments.DTOs;

namespace ClinicManagement.Application.Patients.DTOs;

public class PatientProfileDto
{
    public PatientDto Demographics { get; set; } = default!;
    
    // Visit history summary
    public int TotalVisits { get; set; }
    public DateTime? LastVisitDate { get; set; }
    public DateTime? NextAppointmentDate { get; set; }
    

}
