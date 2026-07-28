using FilamentDbApp.Data;
using FilamentDbApp.Models;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FilamentDbApp.Services;

public sealed class DocumentBrandingRendererService
{
    public const string StableJpegAssetName = "3dp-iceland-labs-logo-pdf.jpg";
    private const string BuiltInPngUri =
        "pack://application:,,,/Assets/3dp-iceland-labs-header-logo.png";

    private readonly LocalDatabase _database;

    public DocumentBrandingRendererService(LocalDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public DocumentBrandingRenderAsset Resolve()
    {
        var selected = new DocumentBrandingService(_database).ResolveCustomOrFallback();
        var builtInBytes = LoadBuiltInPngBytes();
        var sourceBytes = selected.Provenance == DocumentBrandingProvenance.Custom
            ? selected.PngBytes
            : builtInBytes;
        var normalized = DocumentBrandingService.NormalizePng(sourceBytes);
        var provenance = selected.Provenance;
        var status = provenance switch
        {
            DocumentBrandingProvenance.Custom => "Custom document branding selected.",
            DocumentBrandingProvenance.Fallback =>
                "Fallback document branding selected because custom state is invalid.",
            _ => "Built-in document branding selected."
        };
        return BuildRenderAsset(
            provenance,
            normalized.Bytes,
            normalized.Sha256,
            normalized.Width,
            normalized.Height,
            status,
            new DocumentBrandIdentityService(_database).Resolve());
    }

    internal static DocumentBrandingRenderAsset BuildRenderAsset(
        DocumentBrandingProvenance provenance,
        byte[] normalizedPng,
        string sourceSha256,
        int width,
        int height,
        string status,
        string brandDisplayName)
    {
        ArgumentNullException.ThrowIfNull(normalizedPng);
        var jpeg = ConvertPngToWhiteBackgroundJpeg(normalizedPng);
        return new DocumentBrandingRenderAsset(
            provenance,
            normalizedPng,
            "data:image/png;base64," + Convert.ToBase64String(normalizedPng),
            jpeg,
            sourceSha256,
            Convert.ToHexString(SHA256.HashData(jpeg)),
            width,
            height,
            status,
            brandDisplayName);
    }

    private static byte[] ConvertPngToWhiteBackgroundJpeg(byte[] pngBytes)
    {
        using var source = new MemoryStream(pngBytes, writable: false);
        var decoder = new PngBitmapDecoder(
            source,
            BitmapCreateOptions.PreservePixelFormat,
            BitmapCacheOption.OnLoad);
        var converted = new FormatConvertedBitmap(
            decoder.Frames[0],
            PixelFormats.Bgra32,
            null,
            0);
        converted.Freeze();

        var bgraStride = checked(converted.PixelWidth * 4);
        var bgra = new byte[checked(bgraStride * converted.PixelHeight)];
        converted.CopyPixels(bgra, bgraStride, 0);
        var bgrStride = checked(converted.PixelWidth * 3);
        var bgr = new byte[checked(bgrStride * converted.PixelHeight)];
        var targetIndex = 0;
        for (var sourceIndex = 0;
             sourceIndex < bgra.Length;
             sourceIndex += 4, targetIndex += 3)
        {
            var alpha = bgra[sourceIndex + 3];
            bgr[targetIndex] = BlendAgainstWhite(bgra[sourceIndex], alpha);
            bgr[targetIndex + 1] = BlendAgainstWhite(bgra[sourceIndex + 1], alpha);
            bgr[targetIndex + 2] = BlendAgainstWhite(bgra[sourceIndex + 2], alpha);
        }

        var bitmap = BitmapSource.Create(
            converted.PixelWidth,
            converted.PixelHeight,
            96,
            96,
            PixelFormats.Bgr24,
            null,
            bgr,
            bgrStride);
        bitmap.Freeze();
        var encoder = new JpegBitmapEncoder { QualityLevel = 92 };
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var output = new MemoryStream();
        encoder.Save(output);
        return output.ToArray();
    }

    private static byte BlendAgainstWhite(byte component, byte alpha) =>
        (byte)((component * alpha + byte.MaxValue * (byte.MaxValue - alpha) + 127) /
               byte.MaxValue);

    private static byte[] LoadBuiltInPngBytes()
    {
        var resource = Application.GetResourceStream(new Uri(BuiltInPngUri, UriKind.Absolute));
        if (resource?.Stream is null)
            throw new FileNotFoundException("Built-in document branding PNG is unavailable.");
        using (resource.Stream)
        using (var memory = new MemoryStream())
        {
            resource.Stream.CopyTo(memory);
            return memory.ToArray();
        }
    }
}

public sealed record DocumentBrandingRenderAsset(
    DocumentBrandingProvenance Provenance,
    byte[] PngBytes,
    string PngDataUri,
    byte[] JpegBytes,
    string SourceSha256,
    string JpegSha256,
    int PixelWidth,
    int PixelHeight,
    string Status,
    string BrandDisplayName);
