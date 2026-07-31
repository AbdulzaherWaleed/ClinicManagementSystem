using ClinicManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;


namespace ClinicManagement.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _rootPath = configuration["FileStorage:LocalRootPath"] ?? "wwwroot/uploads";
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> SaveFileAsync(
        Stream fileStream, string fileName, string subFolder, CancellationToken cancellationToken = default)
    {
        var folder = Path.Combine(_rootPath, subFolder);
        Directory.CreateDirectory(folder);

        var uniqueName = $"{Guid.NewGuid()}_{fileName}";
        var fullPath = Path.Combine(folder, uniqueName);

        await using var output = File.Create(fullPath);
        await fileStream.CopyToAsync(output, cancellationToken);

        return Path.Combine(subFolder, uniqueName).Replace("\\", "/");
    }

    public Task DeleteFileAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, storagePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    public Task<Stream> GetFileAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, storagePath);
        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult(stream);
    }
}