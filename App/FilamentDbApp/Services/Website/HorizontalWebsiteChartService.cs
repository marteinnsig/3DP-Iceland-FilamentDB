using System.Text.RegularExpressions;

namespace FilamentDbApp.Services.Website;

/// <summary>Versioned presentation transform over the active SQLite template; never changes source data or the saved template.</summary>
public static class HorizontalWebsiteChartService
{
    private const string Marker = "/* 3DP-HORIZONTAL-BARS-v67.0.1 */";

    public static bool VerifyContract()
    {
        const string source = "const DATA={sentinel:42};function renderGrouped(){oldGrouped();}" +
            "function renderSingle(){oldSingle();}function rowScore(){return 42;}" +
            "function renderCombined(){oldCombined();}function cvPartsFromRows(){return 9;}" +
            "function renderConsistency(){oldConsistency();}function strengthConsistencyRows(){return 7;}";
        var result = Apply(source);
        var driftRejected = false;
        try { Apply(source.Replace("function rowScore", "function unsupported", StringComparison.Ordinal)); }
        catch (InvalidOperationException) { driftRejected = true; }
        return driftRejected && Apply(result) == result && result.Contains("const DATA={sentinel:42};", StringComparison.Ordinal) &&
            result.Contains("function rowScore(){return 42;}", StringComparison.Ordinal) &&
            !result.Contains("oldGrouped", StringComparison.Ordinal) && !result.Contains("oldSingle", StringComparison.Ordinal) &&
            !result.Contains("oldCombined", StringComparison.Ordinal) && !result.Contains("oldConsistency", StringComparison.Ordinal);
    }

    public static string Apply(string html)
    {
        if (html.Contains(Marker, StringComparison.Ordinal)) return html;
        using var stream = typeof(HorizontalWebsiteChartService).Assembly.GetManifestResourceStream(
            "FilamentDbApp.Assets.Website.HorizontalBarCharts.js")
            ?? throw new InvalidOperationException("The horizontal chart presentation resource is unavailable.");
        using var reader = new System.IO.StreamReader(stream);
        var script = reader.ReadToEnd();
        var helper = script[..script.IndexOf("// BEGIN renderGrouped", StringComparison.Ordinal)];
        var boundaries = new[] { ("renderGrouped", "renderSingle"), ("renderSingle", "rowScore"),
            ("renderCombined", "cvPartsFromRows"), ("renderConsistency", "strengthConsistencyRows") };
        foreach (var (name, next) in boundaries)
        {
            var replacement = Regex.Match(script, $@"// BEGIN {name}\s*([\s\S]*?)// END {name}").Groups[1].Value;
            var pattern = $@"function {name}\([^)]*\)\{{[\s\S]*?(?=function {next}\()";
            if (string.IsNullOrWhiteSpace(replacement) || Regex.Matches(html, pattern).Count != 1)
                throw new InvalidOperationException($"The active website template does not support horizontal {name} charts.");
            html = Regex.Replace(html, pattern, _ => (name == "renderGrouped" ? Marker + "\n" + helper : "") + replacement);
        }
        return html;
    }
}
