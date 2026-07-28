namespace FilamentDbApp.Models;

public sealed record DocumentBrandingRecord(
    bool CustomLogoEnabled,
    byte[] NormalizedPng,
    string Sha256,
    int PixelWidth,
    int PixelHeight,
    long ByteLength,
    bool HasTransparency,
    string UpdatedAtUtc);

public enum DocumentBrandingProvenance
{
    Default,
    Custom,
    Fallback
}

public sealed record DocumentBrandingSnapshot(
    DocumentBrandingProvenance Provenance,
    byte[] PngBytes,
    string Sha256,
    int PixelWidth,
    int PixelHeight,
    bool HasTransparency,
    string Status);
