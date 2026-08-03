using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Doctors;
using MediatR;

namespace ClinicManagement.Application.Doctors.Commands.UploadDoctorLicense;

public record UploadDoctorLicenseCommand(Guid DoctorId, string? LicenseNumber, DateTime ExpiryDate, string OriginalFileName, Stream FileStream) : IRequest<Guid>;

public class UploadDoctorLicenseCommandHandler : IRequestHandler<UploadDoctorLicenseCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public UploadDoctorLicenseCommandHandler(IApplicationDbContext context, IFileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<Guid> Handle(UploadDoctorLicenseCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _context.Doctors.FindAsync(new object[] { request.DoctorId }, cancellationToken);
        if (doctor == null) throw new Exception("Doctor not found");

        // 1. File Size Validation (Max 5MB)
        if (request.FileStream.Length > 5 * 1024 * 1024)
            throw new Exception("File size exceeds the 5MB limit.");

        // 2. Magic Bytes Validation
        if (!ClinicManagement.Application.Common.Helpers.FileValidationHelper.IsValidPdfOrImage(request.FileStream))
            throw new Exception("Invalid file type. Only PDF and Image files (JPEG, PNG) are allowed.");

        // 3. File Name Handling (Path Traversal Protection)
        string safeFileName = System.IO.Path.GetFileName(request.OriginalFileName);
        string ext = System.IO.Path.GetExtension(safeFileName).ToLower();
        string fileType = ext == ".pdf" ? "PDF" : "Image";
        
        string filePath = await _fileStorage.SaveFileAsync(request.FileStream, safeFileName, "licenses", cancellationToken);

        var license = new DoctorLicense
        {
            DoctorId = request.DoctorId,
            LicenseNumber = request.LicenseNumber,
            ExpiryDate = request.ExpiryDate,
            OriginalFileName = safeFileName,
            FilePath = filePath,
            FileType = fileType,
            IsActive = true
        };

        _context.DoctorLicenses.Add(license);
        await _context.SaveChangesAsync(cancellationToken);

        return license.Id;
    }
}
