using MediatR;

namespace ClinicManagement.Application.Patients.Commands.DeletePatient;

public class DeletePatientCommand : IRequest
{
    public Guid Id { get; set; }
}