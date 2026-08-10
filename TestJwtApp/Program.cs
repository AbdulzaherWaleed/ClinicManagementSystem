using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

class Program {
    static void Main() {
        var claims = new List<Claim> {
            new Claim("Role", "SystemAdministrator"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim("Permissions", "Patients.Edit")
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("a_very_long_secret_key_that_is_at_least_256_bits_long_1234567890"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken("ClinicManagement", "ClinicManagement.UI", claims, null, DateTime.UtcNow.AddHours(1), credentials);
        var handler = new JwtSecurityTokenHandler();
        Console.WriteLine(handler.WriteToken(token));
    }
}
