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
    string Reinforcement,
    double? ThermalResultTemperatureC = null,
    double? ThermalScore = null,
    string ThermalMethodVersion = "",
    string ThermalLimitation = "");

public sealed record OpenAiPilotInput(
    string Template,
    string PlanningNote,
    IReadOnlyList<OpenAiPilotMaterial> Materials);

public sealed record OpenAiPilotPreview(
    string Model,
    string RequestBodyJson,
    string RequestSha256,
    IReadOnlySet<string> AllowedMaterialIds,
    int SourceMaterialIdCount)
{
    public int OmittedMaterialIdCount =>
        Math.Max(0, SourceMaterialIdCount - AllowedMaterialIds.Count);
}

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

public enum OpenAiPilotOutcome
{
    Completed,
    Cancelled,
    TimedOut,
    ApiError,
    ValidationError,
    TransportError
}

public sealed record OpenAiPilotOperationalEvidence(
    DateTimeOffset StartedAtUtc,
    DateTimeOffset CompletedAtUtc,
    long ElapsedMilliseconds,
    OpenAiPilotOutcome Outcome,
    string RequestedModel,
    string PayloadSchema,
    string PromptVersion,
    string RequestSha256,
    int MaterialCount,
    int InputTokens,
    int OutputTokens,
    string ClientRequestId,
    string ServerRequestId,
    string ErrorCategory,
    string CostStatus)
{
    public int TotalTokens => InputTokens + OutputTokens;
}

public sealed record OpenAiPilotExecution(
    OpenAiPilotResult Result,
    OpenAiPilotOperationalEvidence Evidence);

public sealed class OpenAiPilotExecutionException : InvalidOperationException
{
    public OpenAiPilotExecutionException(
        string message,
        OpenAiPilotOperationalEvidence evidence,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Evidence = evidence;
    }

    public OpenAiPilotOperationalEvidence Evidence { get; }
}

public sealed class OpenAiAssistantPilotService
{
    public const string Endpoint = "https://api.openai.com/v1/responses";
    public const string PayloadSchema = "3dpiceland.openai-material-pilot.v1";
    public const string PromptVersion = "v61.0.6-thermal-advisory-v3";
    public const int MaximumMaterials = 40;
    public const int MaximumPlanningNoteCharacters = 2000;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private static readonly HttpClient SharedClient = new(new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.All
    })
    {
        Timeout = Timeout.InfiniteTimeSpan
    };
    private readonly HttpClient _client;
    private readonly TimeProvider _timeProvider;
    private readonly TimeSpan _requestTimeout;
    private readonly Func<string> _clientRequestIdFactory;

    public OpenAiAssistantPilotService(
        HttpClient? client = null,
        TimeProvider? timeProvider = null,
        TimeSpan? requestTimeout = null,
        Func<string>? clientRequestIdFactory = null)
    {
        _client = client ?? SharedClient;
        _timeProvider = timeProvider ?? TimeProvider.System;
        _requestTimeout = requestTimeout ?? TimeSpan.FromSeconds(60);
        _clientRequestIdFactory = clientRequestIdFactory ?? (() => Guid.NewGuid().ToString("D"));
    }

    public OpenAiPilotPreview BuildPreview(string model, OpenAiPilotInput input)
    {
        var distinctMaterials = input.Materials
            .Where(material => !string.IsNullOrWhiteSpace(material.MaterialID))
            .GroupBy(material => material.MaterialID.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(group => Normalize(group.First()))
            .ToList();
        var normalizedMaterials = distinctMaterials
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
        var ids = normalizedMaterials
            .Select(material => material.MaterialID)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var request = new
        {
            model = normalizedModel,
            store = false,
            max_output_tokens = 4000,
            reasoning = new
            {
                effort = "low"
            },
            tools = Array.Empty<object>(),
            instructions =
                "You are a read-only engineering and content-planning assistant for 3DPIceland. " +
                "Use only the supplied material records. Never invent evidence IDs. Treat every recommendation as advisory. " +
                "Thermal values are nearby probe-indicated fixture temperatures from a non-standard comparative method; " +
                "never describe them as ASTM D648, ISO 75, specimen temperature, certified HDT or manufacturer limits. " +
                "Do not request tools, files, URLs, purchasing, inventory, customer, quote, path or credential data.",
            input = governedInputJson,
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "material_advisory",
                    strict = true,
                    schema = BuildResponseSchema(ids)
                }
            }
        };

        var requestJson = JsonSerializer.Serialize(request, JsonOptions);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(requestJson)));
        return new OpenAiPilotPreview(
            normalizedModel,
            requestJson,
            hash,
            ids,
            distinctMaterials.Count);
    }

    public async Task<OpenAiPilotExecution> GenerateAsync(
        OpenAiPilotPreview preview,
        string apiKey,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("No OpenAI API credential is available.");
        }

        var clientRequestId = _clientRequestIdFactory();
        var startedAtUtc = _timeProvider.GetUtcNow();
        var startedTimestamp = _timeProvider.GetTimestamp();
        var serverRequestId = string.Empty;
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey.Trim());
        request.Headers.Add("X-Client-Request-Id", clientRequestId);
        request.Content = new StringContent(preview.RequestBodyJson, Encoding.UTF8, "application/json");

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(_requestTimeout);
        try
        {
            using var response = await _client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                timeout.Token);
            var responseJson = await response.Content.ReadAsStringAsync(timeout.Token);
            serverRequestId = response.Headers.TryGetValues("x-request-id", out var values)
                ? values.FirstOrDefault() ?? string.Empty
                : string.Empty;

            if (!response.IsSuccessStatusCode)
            {
                var safeApiError = BuildSafeApiException(
                    response.StatusCode,
                    responseJson,
                    clientRequestId,
                    serverRequestId);
                throw new OpenAiPilotExecutionException(
                    safeApiError.Message,
                    BuildEvidence(
                        preview,
                        startedAtUtc,
                        startedTimestamp,
                        OpenAiPilotOutcome.ApiError,
                        clientRequestId,
                        serverRequestId,
                        "HTTP " + (int)response.StatusCode),
                    safeApiError);
            }

            OpenAiPilotResult result;
            try
            {
                result = ParseAndValidateResponse(
                    responseJson,
                    preview.AllowedMaterialIds,
                    clientRequestId,
                    serverRequestId);
            }
            catch (Exception ex) when (ex is JsonException or InvalidOperationException or KeyNotFoundException)
            {
                var usage = ReadUsage(responseJson);
                throw new OpenAiPilotExecutionException(
                    $"OpenAI returned output that failed the governed validation contract. " +
                    $"Client request ID: {clientRequestId}; server request ID: {serverRequestId}.",
                    BuildEvidence(
                        preview,
                        startedAtUtc,
                        startedTimestamp,
                        OpenAiPilotOutcome.ValidationError,
                        clientRequestId,
                        serverRequestId,
                        ClassifyValidationFailure(responseJson, ex),
                        usage.InputTokens,
                        usage.OutputTokens),
                    ex);
            }

            var evidence = BuildEvidence(
                preview,
                startedAtUtc,
                startedTimestamp,
                OpenAiPilotOutcome.Completed,
                clientRequestId,
                serverRequestId,
                string.Empty,
                result.InputTokens,
                result.OutputTokens);
            return new OpenAiPilotExecution(result, evidence);
        }
        catch (OpenAiPilotExecutionException)
        {
            throw;
        }
        catch (OperationCanceledException ex)
        {
            var (outcome, category) = ClassifyCancellation(cancellationToken.IsCancellationRequested);
            throw new OpenAiPilotExecutionException(
                outcome == OpenAiPilotOutcome.Cancelled
                    ? "OpenAI request was cancelled. Canonical data is unchanged."
                    : "OpenAI request timed out. Canonical data is unchanged.",
                BuildEvidence(
                    preview,
                    startedAtUtc,
                    startedTimestamp,
                    outcome,
                    clientRequestId,
                    serverRequestId,
                    category),
                ex);
        }
        catch (HttpRequestException ex)
        {
            throw new OpenAiPilotExecutionException(
                $"OpenAI could not be reached. Canonical data is unchanged. Client request ID: {clientRequestId}.",
                BuildEvidence(
                    preview,
                    startedAtUtc,
                    startedTimestamp,
                    OpenAiPilotOutcome.TransportError,
                    clientRequestId,
                    serverRequestId,
                    "HTTP transport"),
                ex);
        }
    }

    internal static (OpenAiPilotOutcome Outcome, string Category) ClassifyCancellation(
        bool callerCancellationRequested) =>
        callerCancellationRequested
            ? (OpenAiPilotOutcome.Cancelled, "Caller cancellation")
            : (OpenAiPilotOutcome.TimedOut, "60-second timeout");

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
            throw new InvalidOperationException(
                $"OpenAI response was not completed. Status: {Limit(status, 80)}.");
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
                        throw new InvalidOperationException("OpenAI declined this request.");
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

    private static object BuildResponseSchema(IReadOnlySet<string> allowedMaterialIds) => new
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
                            items = new
                            {
                                type = "string",
                                @enum = allowedMaterialIds
                                    .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
                                    .ToArray()
                            }
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
        Limit(material.Reinforcement, 120),
        material.ThermalResultTemperatureC,
        material.ThermalScore,
        Limit(material.ThermalMethodVersion, 120),
        Limit(material.ThermalLimitation, 360));

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

    private static (int InputTokens, int OutputTokens) ReadUsage(string responseJson)
    {
        try
        {
            using var response = JsonDocument.Parse(responseJson);
            if (!response.RootElement.TryGetProperty("usage", out var usage))
            {
                return (0, 0);
            }

            return (ReadInt(usage, "input_tokens"), ReadInt(usage, "output_tokens"));
        }
        catch (JsonException)
        {
            return (0, 0);
        }
    }

    private static string ClassifyValidationFailure(string responseJson, Exception exception)
    {
        try
        {
            using var response = JsonDocument.Parse(responseJson);
            var root = response.RootElement;
            var status = ReadString(root, "status");
            if (string.Equals(status, "incomplete", StringComparison.Ordinal))
            {
                var reason = root.TryGetProperty("incomplete_details", out var details)
                    ? ReadString(details, "reason")
                    : string.Empty;
                return string.Equals(reason, "max_output_tokens", StringComparison.Ordinal)
                    ? "Incomplete — max output tokens"
                    : string.Equals(reason, "content_filter", StringComparison.Ordinal)
                        ? "Incomplete — content filter"
                        : "Incomplete response";
            }
        }
        catch (JsonException)
        {
            return "Malformed response envelope";
        }

        if (exception is JsonException)
        {
            return "Malformed structured output";
        }

        if (exception is KeyNotFoundException)
        {
            return "Missing required structured field";
        }

        return exception.Message.Contains("unknown MaterialID", StringComparison.Ordinal)
            ? "Unknown evidence MaterialID"
            : exception.Message.Contains("declined", StringComparison.OrdinalIgnoreCase)
                ? "Provider refusal"
                : exception.Message.Contains("no structured advisory", StringComparison.OrdinalIgnoreCase)
                    ? "Missing structured output"
                    : "Structured response validation";
    }

    private OpenAiPilotOperationalEvidence BuildEvidence(
        OpenAiPilotPreview preview,
        DateTimeOffset startedAtUtc,
        long startedTimestamp,
        OpenAiPilotOutcome outcome,
        string clientRequestId,
        string serverRequestId,
        string errorCategory,
        int inputTokens = 0,
        int outputTokens = 0) =>
        new(
            startedAtUtc,
            _timeProvider.GetUtcNow(),
            Math.Max(
                0,
                (long)Math.Round(
                    _timeProvider.GetElapsedTime(startedTimestamp).TotalMilliseconds,
                    MidpointRounding.AwayFromZero)),
            outcome,
            preview.Model,
            PayloadSchema,
            PromptVersion,
            preview.RequestSha256,
            preview.AllowedMaterialIds.Count,
            inputTokens,
            outputTokens,
            clientRequestId,
            serverRequestId,
            errorCategory,
            "Unavailable — no governed dated pricing snapshot");

    private static Exception BuildSafeApiException(
        HttpStatusCode statusCode,
        string responseJson,
        string clientRequestId,
        string serverRequestId)
    {
        _ = responseJson;
        var safeMessage = statusCode switch
        {
            HttpStatusCode.Unauthorized =>
                "The configured credential was not accepted. Review the OpenAI project key.",
            HttpStatusCode.Forbidden =>
                "The configured project credential does not have permission for this request.",
            (HttpStatusCode)429 =>
                "The OpenAI project rate limit was reached. No automatic retry was attempted.",
            HttpStatusCode.BadRequest =>
                "The governed request was rejected as invalid. Review the model and request contract.",
            _ =>
                "The API returned an error. No raw provider response was retained or displayed."
        };
        return new InvalidOperationException(
            $"OpenAI request failed ({(int)statusCode} {statusCode}). {safeMessage} " +
            $"Client request ID: {clientRequestId}; server request ID: {serverRequestId}.");
    }
}
