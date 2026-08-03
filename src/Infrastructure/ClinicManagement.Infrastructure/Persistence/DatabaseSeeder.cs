using ClinicManagement.Domain.Entities.Identity;
using ClinicManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    private static readonly string[] Roles = ["Admin", "Doctor", "Employee"];

    public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(DatabaseSeeder));
        logger.LogWarning("STARTING DATABASE SEEDER EXECUTION...");
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                logger.LogInformation("Created identity role {Role}", role);
            }
        }

        // Seed 500 patients & appointments
        // await SeedFakeDataAsync(serviceProvider, logger); // Removed for Production

        var adminEmail = configuration["Seed:AdminEmail"] ?? "admin@clinic.local";
        var adminPassword = configuration["Seed:AdminPassword"] ?? "Abdo12345";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin is not null)
        {
            if (!await userManager.CheckPasswordAsync(existingAdmin, adminPassword))
            {
                var removeResult = await userManager.RemovePasswordAsync(existingAdmin);
                if (removeResult.Succeeded)
                {
                    var addResult = await userManager.AddPasswordAsync(existingAdmin, adminPassword);
                    if (addResult.Succeeded)
                    {
                        logger.LogInformation("Reset default admin password");
                    }
                    else
                    {
                        logger.LogError("Failed to add new admin password: {Errors}", string.Join("; ", addResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogError("Failed to remove old admin password: {Errors}", string.Join("; ", removeResult.Errors.Select(e => e.Description)));
                }
            }
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "System Administrator",
            Role = UserRole.Admin,
            IsActive = true
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (!result.Succeeded)
        {
            logger.LogWarning(
                "Default admin user was not created: {Errors}",
                string.Join("; ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(admin, "Admin");
        logger.LogInformation("Seeded default admin user {Email}", adminEmail);
    }

    private static async Task SeedFakeDataAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        var context = serviceProvider.GetRequiredService<ClinicManagement.Application.Common.Interfaces.IApplicationDbContext>();
        
        if (context.Patients.Count() >= 500)
        {
            logger.LogInformation("Patients already seeded (>=500), skipping fake data seed.");
            return;
        }

        logger.LogInformation("Seeding 500 fake patients and appointments...");

        var doctors = context.Doctors.ToList();
        if (!doctors.Any())
        {
            logger.LogWarning("No doctors found to assign appointments to. Skipping fake data seed.");
            return;
        }

        var patients = new List<ClinicManagement.Domain.Entities.Patients.Patient>();
        var appointments = new List<ClinicManagement.Domain.Entities.Appointments.Appointment>();
        var random = new Random();

        var stages = new[] { VisitStage.Checkup, VisitStage.Consultation, VisitStage.Procedure };
        var statuses = new[] { AppointmentStatus.Scheduled, AppointmentStatus.Completed, AppointmentStatus.NoShow };
        var visitTypes = new[] { "General", "Follow-up", "Consultation", "Emergency" };

        for (int i = 1; i <= 500; i++)
        {
            var patient = new ClinicManagement.Domain.Entities.Patients.Patient
            {
                Id = Guid.NewGuid(),
                FullName = $"Test Patient {i}",
                PhoneNumber = $"010{random.Next(10000000, 99999999)}",
                DateOfBirth = DateTime.UtcNow.AddYears(-random.Next(18, 70)),
                Gender = random.Next(2) == 0 ? "Male" : "Female",
                Address = $"Test Address {i}",
                CreatedAt = DateTime.UtcNow
            };
            patients.Add(patient);

            var doctor = doctors[random.Next(doctors.Count)];
            var scheduledStart = DateTime.UtcNow.AddDays(random.Next(-30, 30)).AddHours(random.Next(9, 17));

            var appointment = new ClinicManagement.Domain.Entities.Appointments.Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                ScheduledStart = scheduledStart,
                ScheduledEnd = scheduledStart.AddMinutes(30),
                Status = statuses[random.Next(statuses.Length)],
                VisitStage = stages[random.Next(stages.Length)],
                VisitType = visitTypes[random.Next(visitTypes.Length)],
                Reason = "Test Appointment",
                CreatedAt = DateTime.UtcNow,
                CreatedByEmployeeId = Guid.Empty // Or an admin ID
            };
            appointments.Add(appointment);
        }

        context.Patients.AddRange(patients);
        context.Appointments.AddRange(appointments);
        await context.SaveChangesAsync(CancellationToken.None);

        logger.LogInformation("Successfully seeded 500 fake patients and appointments.");
    }
}
