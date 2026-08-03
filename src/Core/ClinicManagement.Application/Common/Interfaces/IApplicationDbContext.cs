using ClinicManagement.Domain.Entities.Appointments;
using ClinicManagement.Domain.Entities.Doctors;
using ClinicManagement.Domain.Entities.Inventory;
using ClinicManagement.Domain.Entities.Patients;
using ClinicManagement.Domain.Entities.Settings;
using ClinicManagement.Domain.Entities.Staff;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Doctor> Doctors { get; }
    DbSet<DoctorSpecialty> DoctorSpecialties { get; }
    DbSet<DoctorDocument> DoctorDocuments { get; }
    DbSet<Employee> Employees { get; }
    DbSet<EmployeeDoctorAssignment> EmployeeDoctorAssignments { get; }
    DbSet<Patient> Patients { get; }
    DbSet<PatientVisit> PatientVisits { get; }
    DbSet<Appointment> Appointments { get; }
    DbSet<AppointmentStatusHistory> AppointmentStatusHistories { get; }
    DbSet<ClinicSetting> ClinicSettings { get; }
    DbSet<DoctorCustomization> DoctorCustomizations { get; }
    DbSet<InventoryCategory> InventoryCategories { get; }
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<StockTransaction> StockTransactions { get; }
    DbSet<ClinicManagement.Domain.Entities.Identity.UserRefreshToken> UserRefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}