namespace FilamentDbApp.Services.Website;

public sealed class WebsiteBrandingAssetService
{
    public const string Marker = "3DP-CANONICAL-WEBSITE-BRANDING-v57.0.4";
    public const string LogoRelativePath = "assets/3dp-iceland-labs-header-logo.png";
    public const string FaviconRelativePath = "favicon.ico";

    public WebsiteBrandingStageResult Stage(string websiteRoot)
    {
        var logoBytes = LoadPackResource("pack://application:,,,/Assets/3dp-iceland-labs-header-logo.png");
        var faviconSource = FindOutputAsset("3dp-iceland-labs-icon.ico");
        var faviconBytes = IOFile.ReadAllBytes(faviconSource);
        if (logoBytes.Length == 0 || faviconBytes.Length == 0)
            throw new InvalidOperationException("Canonical website logo or favicon asset is empty.");

        var logoPath = IOPath.Combine(websiteRoot, LogoRelativePath.Replace('/', IOPath.DirectorySeparatorChar));
        var faviconPath = IOPath.Combine(websiteRoot, FaviconRelativePath);
        SafeFileOperations.WriteAllBytesAtomic(logoPath, logoBytes);
        SafeFileOperations.WriteAllBytesAtomic(faviconPath, faviconBytes);
        return new WebsiteBrandingStageResult(logoPath, faviconPath, logoBytes.Length, faviconBytes.Length);
    }

    public string Apply(string html)
    {
        if (html.Contains(Marker, StringComparison.Ordinal)) return html;
        var head = $"""
            <!-- {Marker} -->
            <link rel="icon" href="{FaviconRelativePath}" sizes="any">
            """;
        var headClose = html.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
        html = headClose >= 0 ? html.Insert(headClose, head + Environment.NewLine) : head + Environment.NewLine + html;

        var brandedHeader = $"""
            <header class="website-canonical-header"><div class="website-canonical-title">
              <img src="{LogoRelativePath}" alt="3DPIceland Labs">
              <h1>Filament Testing Database</h1>
            </div>
            """;
        html = System.Text.RegularExpressions.Regex.Replace(
            html,
            @"<header>\s*<h1>\s*3DPIceland Labs\s+[–-]\s+Filament Testing Database\s*</h1>",
            _ => brandedHeader,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase |
            System.Text.RegularExpressions.RegexOptions.CultureInvariant);
        const string brandingCss = """
            <style id="canonicalWebsiteBrandingStyles">
            .website-canonical-title{display:grid;grid-template-columns:minmax(190px,260px) minmax(0,1fr);align-items:center;
              gap:26px}.website-canonical-title img{display:block;width:100%;max-height:118px;object-fit:contain;border-radius:16px;
              background:#fff;border:1px solid rgba(148,163,184,.36)}
            .website-canonical-title h1{margin:0;font-size:clamp(34px,5vw,58px);line-height:1.08}
            .portal-navigation-inner{flex-wrap:wrap;overflow-x:visible}
            @media(max-width:700px){.website-canonical-title{grid-template-columns:minmax(120px,170px) minmax(0,1fr);gap:14px}
              .website-canonical-title img{max-height:82px}.website-canonical-title h1{font-size:clamp(25px,7vw,38px)}}
            @media(max-width:460px){.website-canonical-title{grid-template-columns:1fr}.website-canonical-title img{width:170px}}
            </style>
            """;
        headClose = html.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
        return headClose >= 0
            ? html.Insert(headClose, brandingCss + Environment.NewLine)
            : brandingCss + Environment.NewLine + html;
    }

    private static byte[] LoadPackResource(string uri)
    {
        var resource = System.Windows.Application.GetResourceStream(new Uri(uri, UriKind.Absolute))
            ?? throw new InvalidOperationException("Canonical website logo resource is missing.");
        using var stream = resource.Stream;
        using var memory = new System.IO.MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    private static string FindOutputAsset(string name)
    {
        var candidates = new[]
        {
            IOPath.Combine(AppContext.BaseDirectory, "Assets", name),
            IOPath.Combine(AppContext.BaseDirectory, name),
            IOPath.Combine(Environment.CurrentDirectory, "Assets", name)
        };
        return candidates.FirstOrDefault(IOFile.Exists)
            ?? throw new System.IO.FileNotFoundException("Canonical website favicon asset is missing.", name);
    }
}

public sealed record WebsiteBrandingStageResult(
    string LogoPath,
    string FaviconPath,
    int LogoBytes,
    int FaviconBytes);
