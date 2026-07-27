using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FilamentDbApp.Services;

public sealed record OpenAiPilotMaterial(
    string MaterialID,
    string Manufacturer,
    string ProductLine,
    string MarketingName,
    string BaseMaterial,
    string MaterialCategory,
    string VariantFinish,
    string Reinforcement);

public sealed record OpenAiPilotInput(
    string Template,
    string PlanningNote,
    IReadOnlyList<OpenAiPilotMaterial> Materials);

public sealed record OpenAiPilotPreview(
    string Model,
    string RequestBodyJson,
    string RequestSha256,
    IReadOnlySet<string> AllowedMaterialIds);

public sealed record OpenAiPilotFinding(
    string Title,
    string Details,
    IReadOnlyList<string> EvidenceMaterialIds);

public sealed record OpenAiPilotResult(
    string Summary,
    IReadOnlyList<OpenAiPilotFinding> Findings,
    IReadOnlyList<string> Uncertainties,
    IReadOnlyList<string> SuggestedNextActions,
    string ClientRequestId,
    string ServerRequestId,
    int InputTokens,
    int OutputTokens);

public sealed class OpenAiAssistantPilotService
{
    public const string Endpoint = "https://api.openai.com/v1/responses";
    public const string PayloadSchema = "3dpiceland.openai-material-pilot.v1";
    public const string PromptVersion = "v52.2-material-advisory-v1";
    public const int MaximumMaterials = 40;
    public const int MaximumPlanningNoteCharacters = 2000;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private static readonly HttpClient Client = new(new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.All
    })
    {
        Timeout = Timeout.InfiniteTimeSpan
    };

    public OpenAiPilotPreview BuildPreview(string model, OpenAiPilotInput input)
    {
        var normalizedMaterials = input.Materials
            .Where(material => !string.IsNullOrWhiteSpace(material.MaterialID))
            .GroupBy(material => material.MaterialID.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(group => Normalize(group.First()))
            .Take(MaximumMaterials)
            .ToList();

        if (normalizedMaterials.Count == 0)
        {
            throw new InvalidOperationException("At least one canonical MaterialID is required for the OpenAI pilot.");
        }

        var normalizedModel = OpenAiAssistantProviderFoundation.NormalizeModel(model);
        var governedInput = new
        {
            schema = PayloadSchema,
            promptVersion = PromptVersion,
            purpose = "Read-only material and content-planning advisory",
            template = Limit(input.Template, 120),
            planningNote = Limit(input.PlanningNote, MaximumPlanningNoteCharacters),
            materials = normalizedMaterials
        };
        var governedInputJson = JsonSerializer.Serialize(governedInput, JsonOptions);

        var request = new
        {
            model = normalizedModel,
            store = false,
            max_output_tokens = 1800,
            tools = Array.Empty<object>(),
            instructions =
                "You are a read-only engineering and content-planning assistant for 3DPIceland. " +
                "Use only the supplied material records. Never invent evidence IDs. Treat every recommendation as advisory. " +
                "Do not request tools, files, URLs, purchasing, inventory, customer, quote, path or credential data.",
            input = governedInputJson,
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "material_advisory",
                    strict = true,
                    schema = BuildResponseSchema()
                }
            }
        };

        var requestJson = JsonSerializer.Serialize(request, JsonOptions);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(requestJson)));
        var ids = normalizedMaterials
            .Select(material => material.MaterialID)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        return new OpenAiPilotPreview(normalizedModel, requestJson, hash, ids);
    }

    public async Task<OpenAiPilotResult> GenerateAsync(
        OpenAiPilotPreview preview,
        string apiKey,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("No OpenAI API credential is available.");
        }

        var clientRequestId = Guid.NewGuid().ToString("D");
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey.Trim());
        request.Headers.Add("X-Client-Request-Id", clientRequestId);
        request.Content = new StringContent(preview.RequestBodyJson, Encoding.UTF8, "application/json");

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(60));
        using var response = await Client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            timeout.Token);
        var responseJson = await response.Content.ReadAsStringAsync(timeout.Token);
        var serverRequestId = response.Headers.TryGetValues("x-request-id", out var values)
            ? values.FirstOrDefault() ?? string.Empty
            : string.Empty;

        if (!response.IsSuccessStatusCode)
        {
            throw BuildSafeApiException(response.StatusCode, responseJson, clientRequestId, serverRequestId);
        }

        return ParseAndValidateResponse(
            responseJson,
            preview.AllowedMaterialIds,
            clientRequestId,
            serverRequestId);
    }

    public OpenAiPilotResult ParseAndValidateResponse(
        string responseJson,
        IReadOnlySet<string> allowedMaterialIds,
        string clientRequestId,
        string serverRequestId)
    {
        using var response = JsonDocument.Parse(responseJson);
        var root = response.RootElement;
        var status = ReadString(root, "status");
        if (!string.Equals(status, "completed", StringComparison.Ordinal))
        {
            var detail = root.TryGetProperty("incomplete_details", out var incomplete)
                ? incomplete.ToString()
                : "No incomplete detail was returned.";
            throw new InvalidOperationException($"OpenAI response was not completed. Status: {status}; detail: {detail}");
        }

        string? structuredText = null;
        if (root.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in output.EnumerateArray())
            {
                if (!string.Equals(ReadString(item, "type"), "message", StringComparison.Ordinal) ||
                    !item.TryGetProperty("content", out var content) ||
                    content.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var part in content.EnumerateArray())
                {
                    var type = ReadString(part, "type");
                    if (string.Equals(type, "refusal", StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            "OpenAI declined this request: " + ReadString(part, "refusal"));
                    }
                    if (string.Equals(type, "output_text", StringComparison.Ordinal))
                    {
                        structuredText = ReadString(part, "text");
                        break;
                    }
                }
            }
        }

        if (string.IsNullOrWhiteSpace(structuredText))
        {
            throw new InvalidOperationException("OpenAI returned no structured advisory output.");
        }

        using var advisory = JsonDocument.Parse(structuredText);
        var advisoryRoot = advisory.RootElement;
        var findings = new List<OpenAiPilotFinding>();
        foreach (var finding in advisoryRoot.GetProperty("findings").EnumerateArray())
        {
            var evidence = finding.GetProperty("evidenceMaterialIds")
                .EnumerateArray()
                .Select(item => item.GetString()?.Trim() ?? string.Empty)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var unknown = evidence
                .Where(id => !allowedMaterialIds.Contains(id))
                .ToList();
            if (unknown.Count > 0)
            {
                throw new InvalidOperationException(
                    "OpenAI output cited unknown MaterialID value(s): " + string.Join(", ", unknown));
            }

            findings.Add(new OpenAiPilotFinding(
                ReadString(finding, "title"),
                ReadString(finding, "details"),
                evidence));
        }

        var usage = root.TryGetProperty("usage", out var usageElement)
            ? usageElement
            : default;
        return new OpenAiPilotResult(
            ReadString(advisoryRoot, "summary"),
            findings,
            ReadStringArray(advisoryRoot, "uncertainties"),
            ReadStringArray(advisoryRoot, "suggestedNextActions"),
            clientRequestId,
            serverRequestId,
            ReadInt(usage, "input_tokens"),
            ReadInt(usage, "output_tokens"));
    }

    private static object BuildResponseSchema() => new
    {
        type = "object",
        additionalProperties = false,
        properties = new
        {
            summary = new { type = "string" },
            findings = new
            {
                type = "array",
                items = new
                {
                    type = "object",
                    additionalProperties = false,
                    properties = new
                    {
                        title = new { type = "string" },
                        details = new { type = "string" },
                        evidenceMaterialIds = new
                        {
                            type = "array",
                            items = new { type = "string" }
                        }
                    },
                    required = new[] { "title", "details", "evidenceMaterialIds" }
                }
            },
            uncertainties = new
            {
                type = "array",
                items = new { type = "string" }
            },
            suggestedNextActions = new
            {
                type = "array",
                items = new { type = "string" }
            }
        },
        required = new[] { "summary", "findings", "uncertainties", "suggestedNextActions" }
    };

    private static OpenAiPilotMaterial Normalize(OpenAiPilotMaterial material) => new(
        Limit(material.MaterialID, 120),
        Limit(material.Manufacturer, 160),
        Limit(material.ProductLine, 160),
        Limit(material.MarketingName, 200),
        Limit(material.BaseMaterial, 120),
        Limit(material.MaterialCategory, 120),
        Limit(material.VariantFinish, 160),
        Limit(material.Reinforcement, 120));

    private static string Limit(string? value, int maximum) =>
        string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim()[..Math.Min(value.Trim().Length, maximum)];

    private static string ReadString(JsonElement element, string property) =>
        element.ValueKind == JsonValueKind.Object &&
        element.TryGetProperty(property, out var value) &&
        value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? string.Empty
            : string.Empty;

    private static int ReadInt(JsonElement element, string property) =>
        element.ValueKind == JsonValueKind.Object &&
        element.TryGetProperty(property, out var value) &&
        value.TryGetInt32(out var result)
            ? result
            : 0;

    private static IReadOnlyList<string> ReadStringArray(JsonElement element, string property) =>
        element.TryGetProperty(property, out var values) && values.ValueKind == JsonValueKind.Array
            ? values.EnumerateArray()
                .Where(value => value.ValueKind == JsonValueKind.String)
                .Select(value => value.GetString() ?? string.Empty)
                .ToList()
            : Array.Empty<string>();

    private static Exception BuildSafeApiException(
        HttpStatusCode statusCode,
        string responseJson,
        string clientRequestId,
        string serverRequestId)
    {
        var message = string.Empty;
        try
        {
            using var document = JsonDocument.Parse(responseJson);
            if (document.RootElement.TryGetProperty("error", out var error))
            {
                message = ReadString(error, "message");
            }
        }
        catch (JsonException)
        {
            // Never include an unparsed response body in diagnostics.
        }

        var safeMessage = string.IsNullOrWhiteSpace(message)
            ? "The API returned an error without a safe readable message."
            : Limit(message, 500);
        return new InvalidOperationException(
            $"OpenAI request failed ({(int)statusCode} {statusCode}). {safeMessage} " +
            $"Client request ID: {clientRequestId}; server request ID: {serverRequestId}.");
    }
}
