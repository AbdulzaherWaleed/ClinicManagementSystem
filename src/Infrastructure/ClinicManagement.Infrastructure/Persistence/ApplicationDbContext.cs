using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Appointments;
using ClinicManagement.Domain.Entities.Doctors;
using ClinicManagement.Domain.Entities.Identity;
using ClinicManagement.Domain.Entities.Inventory;
using ClinicManagement.Domain.Entities.Patients;
using ClinicManagement.Domain.Entities.Settings;
using ClinicManagement.Domain.Entities.Staff;
using ClinicManagement.Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Persistence;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntitySaveChangesInterceptor auditInterceptor) : base(options)
    {
        _auditInterceptor = auditInterceptor;
    }

    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<DoctorSpecialty> DoctorSpecialties => Set<DoctorSpecialty>();
    public DbSet<DoctorDocument> DoctorDocuments => Set<DoctorDocument>();
    public DbSet<DoctorLicense> DoctorLicenses => Set<DoctorLicense>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeDoctorAssignment> EmployeeDoctorAssignments => Set<EmployeeDoctorAssignment>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientVisit> PatientVisits => Set<PatientVisit>();
    public DbSet<PatientDocument> PatientDocuments => Set<PatientDocument>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AppointmentStatusHistory> AppointmentStatusHistories => Set<AppointmentStatusHistory>();
    public DbSet<ClinicSetting> ClinicSettings => Set<ClinicSetting>();
    public DbSet<DoctorCustomization> DoctorCustomizations => Set<DoctorCustomization>();
    public DbSet<InventoryCategory> InventoryCategories => Set<InventoryCategory>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Ignore<Domain.Common.DomainEvent>();

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor);
    }
}