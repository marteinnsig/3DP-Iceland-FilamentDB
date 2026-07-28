using FilamentDbApp.Data;
using FilamentDbApp.Models;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FilamentDbApp.Services;

public sealed class DocumentBrandingService
{
    public const int MaximumEncodedBytes = 5 * 1024 * 1024;
    public const int MinimumDimension = 16;
    public const int MaximumDimension = 4096;
    public const long MaximumDecodedPixels = 16_000_000;
    public const string CacheRelativePath = "DocumentBranding/document-logo.png";
    private static readonly byte[] PngSignature = [137, 80, 78, 71, 13, 10, 26, 10];

    private readonly LocalDatabase _database;

    public DocumentBrandingService(LocalDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public DocumentBrandingRecord ImportCustomLogo(string sourcePath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
            throw new ArgumentException("Select a PNG file.", nameof(sourcePath));
        var fullSourcePath = IOPath.GetFullPath(sourcePath);
        var sourceBytes = IOFile.ReadAllBytes(fullSourcePath);
        var normalized = NormalizePng(sourceBytes);
        var record = new DocumentBrandingRecord(
            true,
            normalized.Bytes,
            normalized.Sha256,
            normalized.Width,
            normalized.Height,
            normalized.Bytes.LongLength,
            normalized.HasTransparency,
            DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
        _database.SaveDocumentBranding(record);
        MaterializeCache(record);
        return record;
    }

    public DocumentBrandingSnapshot ResolveCustomOrFallback()
    {
        var record = _database.LoadDocumentBranding();
        if (record is null || !record.CustomLogoEnabled)
            return EmptySnapshot(DocumentBrandingProvenance.Default, "Built-in document branding selected.");
        try
        {
            var storedHash = Convert.ToHexString(SHA256.HashData(record.NormalizedPng));
            var normalized = NormalizePng(record.NormalizedPng);
            if (!string.Equals(storedHash, record.Sha256, StringComparison.OrdinalIgnoreCase) ||
                normalized.Width != record.PixelWidth ||
                normalized.Height != record.PixelHeight ||
                normalized.Bytes.LongLength != record.ByteLength)
                throw new InvalidDataException("Stored document logo metadata does not match its decoded content.");
            MaterializeCache(record);
            return new DocumentBrandingSnapshot(
                DocumentBrandingProvenance.Custom,
                record.NormalizedPng,
                record.Sha256,
                record.PixelWidth,
                record.PixelHeight,
                record.HasTransparency,
                "Custom document logo is valid.");
        }
        catch
        {
            TryRemoveCache();
            return EmptySnapshot(
                DocumentBrandingProvenance.Fallback,
                "Custom document logo is missing or corrupt; built-in branding will be used.");
        }
    }

    public void RestoreDefault()
    {
        _database.ClearDocumentBranding();
        TryRemoveCache();
    }

    public string CachePath =>
        IOPath.Combine(_database.DatabaseFolder, CacheRelativePath.Replace('/', IOPath.DirectorySeparatorChar));

    internal static NormalizedDocumentLogo NormalizePng(byte[] sourceBytes)
    {
        ArgumentNullException.ThrowIfNull(sourceBytes);
        if (sourceBytes.Length < PngSignature.Length ||
            !sourceBytes.AsSpan(0, PngSignature.Length).SequenceEqual(PngSignature))
            throw new InvalidDataException("The selected file is not a PNG image.");
        if (sourceBytes.Length > MaximumEncodedBytes)
            throw new InvalidDataException($"PNG files must be {MaximumEncodedBytes / 1024 / 1024} MiB or smaller.");

        try
        {
            using var source = new MemoryStream(sourceBytes, writable: false);
            var decoder = new PngBitmapDecoder(
                source,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);
            if (decoder.Frames.Count != 1)
                throw new InvalidDataException("The PNG must contain exactly one image frame.");
            var frame = decoder.Frames[0];
            ValidateDimensions(frame.PixelWidth, frame.PixelHeight);
            var converted = new FormatConvertedBitmap(frame, PixelFormats.Bgra32, null, 0);
            converted.Freeze();
            var stride = checked(converted.PixelWidth * 4);
            var pixels = new byte[checked(stride * converted.PixelHeight)];
            converted.CopyPixels(pixels, stride, 0);
            var hasTransparency = false;
            for (var index = 3; index < pixels.Length; index += 4)
            {
                if (pixels[index] == byte.MaxValue) continue;
                hasTransparency = true;
                break;
            }

            var cleanFrame = BitmapFrame.Create(converted, null, null, null);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(cleanFrame);
            using var output = new MemoryStream();
            encoder.Save(output);
            var normalizedBytes = output.ToArray();
            if (normalizedBytes.Length > MaximumEncodedBytes)
                throw new InvalidDataException("The normalized PNG exceeds the governed size limit.");
            return new NormalizedDocumentLogo(
                normalizedBytes,
                Convert.ToHexString(SHA256.HashData(normalizedBytes)),
                converted.PixelWidth,
                converted.PixelHeight,
                hasTransparency);
        }
        catch (InvalidDataException)
        {
            throw;
        }
        catch (Exception ex) when (ex is NotSupportedException or FileFormatException or OverflowException or ArgumentException)
        {
            throw new InvalidDataException("The PNG could not be decoded safely.", ex);
        }
    }

    private static void ValidateDimensions(int width, int height)
    {
        if (width is < MinimumDimension or > MaximumDimension ||
            height is < MinimumDimension or > MaximumDimension)
            throw new InvalidDataException(
                $"PNG dimensions must be between {MinimumDimension} and {MaximumDimension} pixels.");
        if ((long)width * height > MaximumDecodedPixels)
            throw new InvalidDataException($"PNG decoded pixels must not exceed {MaximumDecodedPixels:N0}.");
    }

    private void MaterializeCache(DocumentBrandingRecord record)
    {
        var cachePath = IOPath.GetFullPath(CachePath);
        var allowedRoot = IOPath.GetFullPath(_database.DatabaseFolder)
            .TrimEnd(IOPath.DirectorySeparatorChar, IOPath.AltDirectorySeparatorChar) +
            IOPath.DirectorySeparatorChar;
        if (!cachePath.StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Document branding cache path escaped the database folder.");
        SafeFileOperations.WriteAllBytesAtomic(cachePath, record.NormalizedPng);
        var written = IOFile.ReadAllBytes(cachePath);
        if (!string.Equals(
                Convert.ToHexString(SHA256.HashData(written)),
                record.Sha256,
                StringComparison.OrdinalIgnoreCase))
            throw new IOException("The governed document logo cache failed SHA-256 verification.");
    }

    private void TryRemoveCache()
    {
        try
        {
            if (IOFile.Exists(CachePath)) IOFile.Delete(CachePath);
        }
        catch
        {
            // Cache cleanup is best effort; SQLite selection remains authoritative.
        }
    }

    private static DocumentBrandingSnapshot EmptySnapshot(
        DocumentBrandingProvenance provenance,
        string status) =>
        new(provenance, Array.Empty<byte>(), string.Empty, 0, 0, false, status);
}

internal sealed record NormalizedDocumentLogo(
    byte[] Bytes,
    string Sha256,
    int Width,
    int Height,
    bool HasTransparency);
