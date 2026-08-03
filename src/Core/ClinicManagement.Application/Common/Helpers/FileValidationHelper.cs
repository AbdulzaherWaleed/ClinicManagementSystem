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
        if (fileStream == null || fileStream.Length < 8) return false;

        var headerBytes = new byte[8];
        long originalPosition = fileStream.Position;
        
        fileStream.Position = 0;
        fileStream.Read(headerBytes, 0, 8);
        fileStream.Position = originalPosition;

        return PdfMagicBytes.Any(sig => headerBytes.Take(sig.Length).SequenceEqual(sig)) ||
               JpegMagicBytes.Any(sig => headerBytes.Take(sig.Length).SequenceEqual(sig)) ||
               PngMagicBytes.Any(sig => headerBytes.Take(sig.Length).SequenceEqual(sig));
    }
}
