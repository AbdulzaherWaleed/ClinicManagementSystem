using ClinicManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;


namespace ClinicManagement.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        var configuredPath = configuration["FileStorage:LocalRootPath"] ?? "App_Data/Uploads";
        _rootPath = Path.IsPathRooted(configuredPath) ? configuredPath : Path.Combine(webHostEnvironment.ContentRootPath, configuredPath);
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> SaveFileAsync(
        Stream fileStream, string fileName, string subFolder, CancellationToken cancellationToken = default)
    {
        var folder = Path.Combine(_rootPath, subFolder);
        Directory.CreateDirectory(folder);

        var safeFileName = Path.GetFileName(fileName)?.Trim();
        if (string.IsNullOrWhiteSpace(safeFileName))
            throw new InvalidOperationException("Invalid file name");

        var uniqueName = $"{Guid.NewGuid()}_{safeFileName}";
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