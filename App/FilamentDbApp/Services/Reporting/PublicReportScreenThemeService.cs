namespace FilamentDbApp.Services.Reporting;

public static class PublicReportScreenThemeService
{
    public const string Marker = "3DP-PUBLIC-REPORT-SCREEN-THEME-v42.14-r3";

    private const string ScreenCss = "@media screen{html{color-scheme:dark}body{background:#0b1020!important;color:#e5e7eb!important}main{background:#111827!important;color:#e5e7eb!important;box-shadow:0 20px 55px rgba(0,0,0,.34)}header{border-color:#3b82f6!important}.card,.note,.entry,.chart,.comparison-chart,.panel,.review{background:#0f172a!important;color:#e5e7eb!important;border-color:#334155!important}table{color:#e5e7eb}th,td{border-color:#334155!important}.meta,.muted,.label,.material-id,footer{color:#94a3b8!important}a{color:#60a5fa!important}.track,.bar-track{background:#334155!important}.fill,.bar-fill{background:#3b82f6!important}.radar-grid{stroke:#64748b!important;stroke-width:1.35!important}.radar-axis{stroke:#94a3b8!important;stroke-width:1.35!important}.radar-label{fill:#cbd5e1!important}.radar-poly-selected{fill:rgba(248,250,252,.12)!important;stroke:#f8fafc!important}.radar-poly-material{fill:rgba(96,165,250,.10)!important;stroke:#60a5fa!important}.radar-poly-manufacturer{fill:rgba(56,189,248,.08)!important;stroke:#38bdf8!important}.legend-item:first-child .legend-line{background:#f8fafc!important}}/* " + Marker + " */";

    public static string Apply(string html)
    {
        if (string.IsNullOrWhiteSpace(html) || html.Contains(Marker, StringComparison.Ordinal)) return html;
        return html.Replace("</style>", ScreenCss + "</style>", StringComparison.Ordinal);
    }
}
