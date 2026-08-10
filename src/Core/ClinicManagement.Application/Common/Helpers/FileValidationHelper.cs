using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ClinicManagement.Application.Common.Helpers;

public static class FileValidationHelper
{
    private static readonly byte[][] PdfMagicBytes = { new byte[] { 0x25, 0x50, 0x44, 0x46 } }; // %PDF
    private static readonly byte[][] JpegMagicBytes = { new byte[] { 0xFF, 0xD8, 0xFF } };
    private static readonly byte[][] PngMagicBytes = { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } };

    public static bool IsValidPdfOrImage(Stream fileStream)
    {
        return GetFileType(fileStream) != "Unknown";
    }

    public static string GetFileType(Stream fileStream)
    {
        if (fileStream == null || fileStream.Length < 8) return "Unknown";

        var headerBytes = new byte[8];
        long originalPosition = fileStream.Position;
        
        fileStream.Position = 0;
        int bytesRead = fileStream.Read(headerBytes, 0, 8);
        fileStream.Position = originalPosition;

        if (bytesRead < 4) return "Unknown";

        if (PdfMagicBytes.Any(sig => headerBytes.Take(sig.Length).SequenceEqual(sig)))
            return "PDF";
            
        if (JpegMagicBytes.Any(sig => headerBytes.Take(sig.Length).SequenceEqual(sig)) ||
            PngMagicBytes.Any(sig => headerBytes.Take(sig.Length).SequenceEqual(sig)))
            return "Image";

        return "Unknown";
    }
}

