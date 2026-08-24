using System.Text.Json;

namespace FilamentDbApp.Services.Website;

public sealed class WebsiteVerificationService
{
    public WebsiteReleaseReadinessResult VerifyReleaseContract(
        string previewHtml,
        string productionHtml,
        string previewRedirectHtml,
        string productionRedirectHtml,
        string previewManifest,
        string productionManifest,
        string whitepaperFileName,
        string expectedBuildLabel)
    {
        var result = new WebsiteReleaseReadinessResult
        {
            PreviewModeMarkerValid = previewHtml.Contains("safe preview file: index-test.html", StringComparison.Ordinal),
            ProductionModeMarkerValid = productionHtml.Contains("production file: index.html", StringComparison.Ordinal),
            BuildIdentityPresent = previewHtml.Contains(expectedBuildLabel, StringComparison.Ordinal) &&
                                   productionHtml.Contains(expectedBuildLabel, StringComparison.Ordinal),
            PortalContractValid = HasPortalContract(previewHtml) && HasPortalContract(productionHtml),
            RedirectContractValid = HasRedirectContract(previewRedirectHtml, "../index-test.html#manufacturers") &&
                                    HasRedirectContract(productionRedirectHtml, "../index.html#manufacturers"),
            PreviewManifestValid = previewManifest.Contains("index-test.html", StringComparison.Ordinal) &&
                                   previewManifest.Contains("manufacturers/index-test.html", StringComparison.Ordinal) &&
                                   previewManifest.Contains("Production index.html and manufacturers/index.html were not overwritten.", StringComparison.Ordinal),
            ProductionManifestValid = productionManifest.Contains("index.html", StringComparison.Ordinal) &&
                                      productionManifest.Contains("manufacturers/index.html", StringComparison.Ordinal) &&
                                      productionManifest.Contains("Previous index.html", StringComparison.Ordinal),
            WhitepaperContractValid = !string.IsNullOrWhiteSpace(whitepaperFileName) &&
                                      previewManifest.Contains(whitepaperFileName, StringComparison.Ordinal) &&
                                      productionManifest.Contains(whitepaperFileName, StringComparison.Ordinal),
            ReportPortalManifestValid = previewManifest.Contains("Public reports portal:", StringComparison.Ordinal) &&
                                        productionManifest.Contains("Public reports portal:", StringComparison.Ordinal) &&
                                        previewManifest.Contains("Public report catalog entries staged:", StringComparison.Ordinal) &&
                                        productionManifest.Contains("Public report catalog entries staged:", StringComparison.Ordinal)
        };

        var previewCanonicalHtml = RemoveGeneratedHeader(previewHtml)
            .Replace(PublicReportWebsiteIntegrationService.PreviewPortalRoute, PublicReportWebsiteIntegrationService.ProductionPortalRoute, StringComparison.Ordinal);
        var productionCanonicalHtml = RemoveGeneratedHeader(productionHtml);
        result.RendererParityValid = previewCanonicalHtml.Equals(productionCanonicalHtml, StringComparison.Ordinal);
        result.Passed = result.PreviewModeMarkerValid &&
                        result.ProductionModeMarkerValid &&
                        result.BuildIdentityPresent &&
                        result.RendererParityValid &&
                        result.PortalContractValid &&
                        result.RedirectContractValid &&
                        result.PreviewManifestValid &&
                        result.ProductionManifestValid &&
                        result.WhitepaperContractValid &&
                        result.ReportPortalManifestValid;

        if (!result.Passed)
        {
            var failedContracts = new List<string>();
            if (!result.PreviewModeMarkerValid) failedContracts.Add("Preview mode marker");
            if (!result.ProductionModeMarkerValid) failedContracts.Add("Production mode marker");
            if (!result.BuildIdentityPresent) failedContracts.Add("build identity");
            if (!result.RendererParityValid) failedContracts.Add(BuildParityFailureDetail(previewCanonicalHtml, productionCanonicalHtml));
            if (!result.PortalContractValid) failedContracts.Add("portal contract");
            if (!result.RedirectContractValid) failedContracts.Add("manufacturer redirect contract");
            if (!result.PreviewManifestValid) failedContracts.Add("Preview manifest");
            if (!result.ProductionManifestValid) failedContracts.Add("Production manifest");
            if (!result.WhitepaperContractValid) failedContracts.Add("whitepaper contract");
            if (!result.ReportPortalManifestValid) failedContracts.Add("public report portal manifest contract");
            result.ErrorMessage = "Failed release contracts: " + string.Join(", ", failedContracts) + ".";
        }
        return result;
    }

    public WebsiteVerificationResult Verify(
        WebsiteChartPayload payload,
        WebsiteRadarVerificationResult radar,
        WebsiteHtmlRendererVerificationResult renderer,
        string? templateHtml,
        string? dataJson)
    {
        var result = new WebsiteVerificationResult
        {
            MaterialRows = payload.Tensile.Count,
            TensileRows = payload.Tensile.Count,
            ImpactRows = payload.Impact.Count,
            StiffnessRows = payload.Stiffness.Count,
            ThermalRows = payload.Thermal.Count,
            RadarSelectedRows = radar.SelectedRadarRows,
            RadarMaterialAverageGroups = radar.MaterialAverageGroups,
            RadarReinforcementAverageGroups = radar.ReinforcementAverageGroups,
            MissingProductUrls = CountMissing(payload.Tensile, "productUrl"),
            MissingYoutubeUrls = CountMissing(payload.Tensile, "youtubeUrl")
        };

        result.HtmlGenerated = !string.IsNullOrWhiteSpace(templateHtml) && !string.IsNullOrWhiteSpace(dataJson);
        result.DataBlockValid = !string.IsNullOrWhiteSpace(dataJson) && dataJson.Contains("\"tensile\"") &&
                                dataJson.Contains("\"impact\"") && dataJson.Contains("\"stiffness\"") &&
                                dataJson.Contains("\"thermal\"");
        result.JsonValid = IsValidJson(dataJson);
        result.RequiredCssPresent = ContainsAny(templateHtml, "<style", "stylesheet");
        result.RequiredJavaScriptPresent = ContainsAny(templateHtml, "<script", "const DATA");
        result.RequiredSectionsPresent = ContainsAll(templateHtml,
            "tensileChart",
            "impactChart",
            "stiffnessChart",
            "thermalChart",
            "Fixture thermal temperature, °C",
            "3DP-THERMAL-PUBLIC-v61.0.7-r3",
            "<th>Stiffness</th><th>Thermal</th><th>Layer adhesion</th>",
            "performanceProfileChart");
        var tensileSection = GetChartSection(templateHtml, "tensileChart");
        var impactSection = GetChartSection(templateHtml, "impactChart");
        result.ChartTerminologyValid =
            ContainsAll(tensileSection, "Tensile Strength", "Layer Adhesion Strength") &&
            !ContainsAny(tensileSection, "Impact Strength Flat", "Impact Strength Upright") &&
            ContainsAll(impactSection, "Impact Strength Flat", "Impact Strength Upright") &&
            !ContainsAny(impactSection, "Tensile Strength", "Layer Adhesion Strength") &&
            ContainsAll(templateHtml,
                "target==='tensileChart'",
                "target==='impactChart'",
                "Impact Strength Flat",
                "Impact Strength Upright");
        result.MasterTemplateIdentityValid = ContainsAll(templateHtml,
            "3DPIceland Website Pricing & Value Platform v36.0",
            "pricingExplorerCard",
            "Performance vs Price",
            "Value Rankings",
            "pricePerformanceChart");
        result.MaterialIdIntegrity = payload.Tensile.All(row => HasText(row, "materialId")) &&
                                     payload.Impact.All(row => HasText(row, "materialId")) &&
                                     payload.Stiffness.All(row => HasText(row, "materialId")) &&
                                     payload.Thermal.All(row => HasText(row, "materialId"));
        result.NoDuplicateMaterialIds = !HasDuplicates(payload.Tensile, "materialId");
        result.ChartPayloadValid = payload.Tensile.Count > 0 &&
                                   payload.Tensile.Count == payload.Impact.Count &&
                                   payload.Tensile.Count == payload.Stiffness.Count &&
                                   payload.Tensile.Count == payload.Thermal.Count &&
                                   payload.Tensile.Any(row => HasAnyNumeric(row, "upright", "flat")) &&
                                   payload.Thermal.Any(row => HasAnyNumeric(row, "value"));
        result.RadarPayloadValid = radar.Passed && radar.SelectedRadarRows == payload.Tensile.Count;
        result.RendererPayloadValid = renderer.Passed;
        result.PublishReady = result.HtmlGenerated &&
                              result.DataBlockValid &&
                              result.JsonValid &&
                              result.RequiredCssPresent &&
                              result.RequiredJavaScriptPresent &&
                              result.RequiredSectionsPresent &&
                              result.ChartTerminologyValid &&
                              result.MasterTemplateIdentityValid &&
                              result.MaterialIdIntegrity &&
                              result.NoDuplicateMaterialIds &&
                              result.ChartPayloadValid &&
                              result.RadarPayloadValid &&
                              result.RendererPayloadValid;
        result.Passed = result.PublishReady;
        return result;
    }

    private static bool IsValidJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;
        try
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.ValueKind == JsonValueKind.Object;
        }
        catch
        {
            return false;
        }
    }

    private static string GetChartSection(string? html, string chartId)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;
        var chartIndex = html.IndexOf($"id=\"{chartId}\"", StringComparison.Ordinal);
        if (chartIndex < 0) return string.Empty;
        var sectionStart = html.LastIndexOf("<section", chartIndex, StringComparison.Ordinal);
        var sectionEnd = html.IndexOf("</section>", chartIndex, StringComparison.Ordinal);
        return sectionStart >= 0 && sectionEnd >= 0
            ? html[sectionStart..(sectionEnd + "</section>".Length)]
            : string.Empty;
    }

    private static string RemoveGeneratedHeader(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;
        return System.Text.RegularExpressions.Regex.Replace(
            html,
            @"<!-- Generated by 3DPIceland Filament DB .*?\. (?:safe preview file: index-test\.html|production file: index\.html)\. -->\r?\n?",
            string.Empty,
            System.Text.RegularExpressions.RegexOptions.CultureInvariant);
    }

    private static string BuildParityFailureDetail(string previewHtml, string productionHtml)
    {
        var sharedLength = Math.Min(previewHtml.Length, productionHtml.Length);
        var firstDifference = 0;
        while (firstDifference < sharedLength && previewHtml[firstDifference] == productionHtml[firstDifference]) firstDifference++;
        return $"renderer parity at character {firstDifference} (Preview {previewHtml.Length} chars, Production {productionHtml.Length} chars)";
    }

    private static bool HasPortalContract(string html)
    {
        return new[]
        {
            "portalPageDatabase",
            "portalPagePricing",
            "portalPageExperimental",
            "portalPageManufacturers",
            PrintingPriceCalculatorPortalService.PortalPageId,
            "portalPageReports",
            "portalPageMethodology",
            PrintingPriceCalculatorPortalService.PortalMarker,
            WebsiteBrandingAssetService.Marker,
            WebsiteBrandingAssetService.LogoRelativePath,
            WebsiteBrandingAssetService.FaviconRelativePath,
            PublicReportWebsiteIntegrationService.WebsitePortalMarker,
            PublicReportWebsiteIntegrationService.EmbeddedPortalMarker,
            "nativePortalNavigationScript"
        }.All(marker => html.Contains(marker, StringComparison.Ordinal));
    }

    private static bool HasRedirectContract(string html, string target)
    {
        return html.Contains($"url={target}", StringComparison.Ordinal) &&
               html.Contains($"window.location.replace(\"{target}\")", StringComparison.Ordinal) &&
               html.Contains($"href=\"{target}\"", StringComparison.Ordinal) &&
               html.Contains("https://iskort.is/3dp/index.html#manufacturers", StringComparison.Ordinal);
    }

    private static bool ContainsAny(string? text, params string[] values)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        return values.Any(value => text.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private static bool ContainsAll(string? text, params string[] values)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        return values.All(value => text.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private static int CountMissing(IReadOnlyList<Dictionary<string, object?>> rows, string key)
    {
        return rows.Count(row => !HasText(row, key));
    }

    private static bool HasText(Dictionary<string, object?> row, string key)
    {
        return row.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value?.ToString());
    }

    private static bool HasDuplicates(IReadOnlyList<Dictionary<string, object?>> rows, string key)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in rows)
        {
            if (!row.TryGetValue(key, out var value)) continue;
            var text = value?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(text)) continue;
            if (!seen.Add(text)) return true;
        }
        return false;
    }

    private static bool HasAnyNumeric(Dictionary<string, object?> row, params string[] keys)
    {
        return keys.Any(key => row.TryGetValue(key, out var value) && IsNumeric(value));
    }

    private static bool IsNumeric(object? value)
    {
        return value switch
        {
            null => false,
            int => true,
            long => true,
            float f => float.IsFinite(f),
            double d => double.IsFinite(d),
            decimal => true,
            _ => double.TryParse(value.ToString(), out var parsed) && double.IsFinite(parsed)
        };
    }
}

public sealed class WebsiteReleaseReadinessResult
{
    public bool Passed { get; set; }
    public bool PreviewModeMarkerValid { get; set; }
    public bool ProductionModeMarkerValid { get; set; }
    public bool BuildIdentityPresent { get; set; }
    public bool RendererParityValid { get; set; }
    public bool PortalContractValid { get; set; }
    public bool RedirectContractValid { get; set; }
    public bool PreviewManifestValid { get; set; }
    public bool ProductionManifestValid { get; set; }
    public bool WhitepaperContractValid { get; set; }
    public bool ReportPortalManifestValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public sealed class WebsiteVerificationResult
{
    public bool Passed { get; set; }
    public bool PublishReady { get; set; }
    public int MaterialRows { get; set; }
    public int TensileRows { get; set; }
    public int ImpactRows { get; set; }
    public int StiffnessRows { get; set; }
    public int ThermalRows { get; set; }
    public int RadarSelectedRows { get; set; }
    public int RadarMaterialAverageGroups { get; set; }
    public int RadarReinforcementAverageGroups { get; set; }
    public int MissingProductUrls { get; set; }
    public int MissingYoutubeUrls { get; set; }
    public bool HtmlGenerated { get; set; }
    public bool DataBlockValid { get; set; }
    public bool JsonValid { get; set; }
    public bool RequiredCssPresent { get; set; }
    public bool RequiredJavaScriptPresent { get; set; }
    public bool RequiredSectionsPresent { get; set; }
    public bool ChartTerminologyValid { get; set; }
    public bool MasterTemplateIdentityValid { get; set; }
    public bool MaterialIdIntegrity { get; set; }
    public bool NoDuplicateMaterialIds { get; set; }
    public bool ChartPayloadValid { get; set; }
    public bool RadarPayloadValid { get; set; }
    public bool RendererPayloadValid { get; set; }
}
