using MediatR;
using System.Text.Json.Serialization;

namespace ClinicManagement.Application.Employees.Commands.UpdateEmployeePermissions;

public class UpdateEmployeePermissionsCommand : IRequest
{
    [JsonIgnore]
    public Guid EmployeeId { get; set; }
    
    public List<string> Permissions { get; set; } = new();
}
