using System.IO;

namespace FilamentDbApp.Services.Reporting;

public sealed class ReportAssetPipelineService
{
    private static readonly string[] RequiredAssetNames =
    {
        "3dp-iceland-labs-logo-pdf.jpg",
        "3dp-iceland-labs-icon.ico"
    };

    public ReportAssetPipelineVerificationResult Verify(bool canonicalHtmlReady, bool pdfExportReady)
    {
        var assets = RequiredAssetNames
            .Select(name => new ReportAssetPipelineItem(name, FindAssetPath(name), IsSharedReportAsset(name)))
            .ToList();

        var pdfLogoAsset = assets.First(asset => asset.Name == "3dp-iceland-labs-logo-pdf.jpg");
        var iconAsset = assets.First(asset => asset.Name == "3dp-iceland-labs-icon.ico");
        var sharedAssetsReady = pdfLogoAsset.Exists;
        var packageManifestReady = canonicalHtmlReady && pdfExportReady && sharedAssetsReady;

        var result = new ReportAssetPipelineVerificationResult
        {
            Owner = "ReportAssetPipelineService",
            RequiredAssets = assets.Count,
            AssetsAvailable = assets.Count(asset => asset.Exists),
            SharedReportAssets = assets.Count(asset => asset.Exists && asset.SharedByHtmlAndPdf),
            LogoAssetAvailable = pdfLogoAsset.Exists,
            PdfLogoAssetAvailable = pdfLogoAsset.Exists,
            IconAssetAvailable = iconAsset.Exists,
            PackageManifestReady = packageManifestReady,
            AssetCopyStrategy = "Copy report assets to output and expose them through the canonical HTML/PDF report package manifest"
        };

        result.Passed = result.AssetsAvailable == result.RequiredAssets &&
                        result.SharedReportAssets >= 1 &&
                        result.LogoAssetAvailable &&
                        result.PdfLogoAssetAvailable &&
                        result.IconAssetAvailable &&
                        result.PackageManifestReady;

        return result;
    }

    private static bool IsSharedReportAsset(string name)
    {
        return name.Contains("logo", StringComparison.OrdinalIgnoreCase);
    }

    private static string FindAssetPath(string name)
    {
        var candidatePaths = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Assets", name),
            Path.Combine(AppContext.BaseDirectory, name),
            Path.Combine(Directory.GetCurrentDirectory(), "Assets", name)
        };

        return candidatePaths.FirstOrDefault(File.Exists) ?? string.Empty;
    }
}

public sealed record ReportAssetPipelineItem(string Name, string Path, bool SharedByHtmlAndPdf)
{
    public bool Exists => !string.IsNullOrWhiteSpace(Path) && File.Exists(Path);
}

public sealed class ReportAssetPipelineVerificationResult
{
    public bool Passed { get; set; }
    public string Owner { get; set; } = string.Empty;
    public int RequiredAssets { get; set; }
    public int AssetsAvailable { get; set; }
    public int SharedReportAssets { get; set; }
    public bool LogoAssetAvailable { get; set; }
    public bool PdfLogoAssetAvailable { get; set; }
    public bool IconAssetAvailable { get; set; }
    public bool PackageManifestReady { get; set; }
    public string AssetCopyStrategy { get; set; } = string.Empty;
}
