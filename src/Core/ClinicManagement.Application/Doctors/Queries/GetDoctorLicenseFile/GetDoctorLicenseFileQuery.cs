using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClinicManagement.Application.Common.Interfaces;
using MediatR;

namespace ClinicManagement.Application.Doctors.Queries.GetDoctorLicenseFile;

public record GetDoctorLicenseFileQuery(Guid DoctorId, Guid LicenseId) : IRequest<(Stream FileStream, string ContentType, string FileName)>;

public class GetDoctorLicenseFileQueryHandler : IRequestHandler<GetDoctorLicenseFileQuery, (Stream FileStream, string ContentType, string FileName)>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public GetDoctorLicenseFileQueryHandler(IApplicationDbContext context, IFileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<(Stream FileStream, string ContentType, string FileName)> Handle(GetDoctorLicenseFileQuery request, CancellationToken cancellationToken)
    {
        var license = await _context.DoctorLicenses.FindAsync(new object[] { request.LicenseId }, cancellationToken);

        if (license == null || license.DoctorId != request.DoctorId || license.IsDeleted)
            throw new Exception("License not found.");

        var stream = await _fileStorage.GetFileAsync(license.FilePath, cancellationToken);

        string contentType = license.FileType == "PDF" ? "application/pdf" : "image/jpeg";
        
        return (stream, contentType, license.OriginalFileName);
    }
}
