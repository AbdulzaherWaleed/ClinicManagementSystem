using MediatR;

namespace ClinicManagement.Application.Appointments.Queries.ExportAppointments;

public class ExportAppointmentsQuery : IRequest<byte[]>
{
    // Optionally include filters here (e.g. date range), but we will export all or recent if not provided
}
