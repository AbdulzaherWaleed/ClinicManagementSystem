using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClinicManagement.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Connection string used ONLY at design-time for generating migrations.
        // The actual runtime connection string still comes from appsettings.json.
        const string connectionString =
            "Server=.;Database=ClinicManagementDb;Trusted_Connection=True;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(
            optionsBuilder.Options,
            new Interceptors.AuditableEntitySaveChangesInterceptor(
                new DesignTimeCurrentUserService(),
                new DesignTimeDateTimeService()));
    }
}

internal class DesignTimeCurrentUserService : ClinicManagement.Application.Common.Interfaces.ICurrentUserService
{
    public Guid? UserId => null;
    public string? Role => null;
    public List<Guid> AssignedDoctorIds => new();
}

internal class DesignTimeDateTimeService : ClinicManagement.Application.Common.Interfaces.IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}