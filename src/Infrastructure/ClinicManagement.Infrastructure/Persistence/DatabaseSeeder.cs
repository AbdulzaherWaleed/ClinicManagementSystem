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
}
