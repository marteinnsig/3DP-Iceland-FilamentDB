namespace FilamentDbApp.Services;

public sealed record AiAssistantProviderConfiguration(string ProviderId, string Model);

public sealed record AiAssistantProviderDiagnostic(
    bool IsReady,
    string ProviderId,
    string Model,
    string Message,
    bool UsedNetwork);

public interface IAiAssistantProvider
{
    string ProviderId { get; }
    bool UsesExternalNetwork { get; }
    AiAssistantProviderDiagnostic Inspect(AiAssistantProviderConfiguration configuration, bool credentialConfigured);
}

public sealed class LocalAiAssistantProvider : IAiAssistantProvider
{
    public const string Id = "local";
    public string ProviderId => Id;
    public bool UsesExternalNetwork => false;

    public AiAssistantProviderDiagnostic Inspect(
        AiAssistantProviderConfiguration configuration,
        bool credentialConfigured) =>
        new(
            true,
            ProviderId,
            string.Empty,
            "Local deterministic provider is ready. No network or API credential is used.",
            false);
}

public sealed class OpenAiAssistantProviderFoundation : IAiAssistantProvider
{
    public const string Id = "openai";
    public const string CredentialTarget = "3DPIceland.FilamentDbApp.AiAssistant.OpenAI";
    public const string DefaultModel = "gpt-5.6-sol";

    public string ProviderId => Id;
    public bool UsesExternalNetwork => true;

    public AiAssistantProviderDiagnostic Inspect(
        AiAssistantProviderConfiguration configuration,
        bool credentialConfigured)
    {
        var model = NormalizeModel(configuration.Model);
        if (!credentialConfigured)
        {
            return new(
                false,
                ProviderId,
                model,
                "OpenAI foundation is selected, but no API credential is stored in Windows Credential Manager.",
                false);
        }

        return new(
            true,
            ProviderId,
            model,
            "OpenAI credential and provider configuration are present. This foundation check used no network and sent no data.",
            false);
    }

    public static string NormalizeModel(string? model) =>
        string.IsNullOrWhiteSpace(model) ? DefaultModel : model.Trim();
}

public sealed class FakeAiAssistantProvider : IAiAssistantProvider
{
    public const string Id = "fake";
    public string ProviderId => Id;
    public bool UsesExternalNetwork => false;

    public AiAssistantProviderDiagnostic Inspect(
        AiAssistantProviderConfiguration configuration,
        bool credentialConfigured) =>
        new(
            true,
            ProviderId,
            "deterministic-fake",
            "Deterministic fake provider is ready; no network or credential is permitted.",
            false);
}

public static class AiAssistantProviderRegistry
{
    public static IAiAssistantProvider Resolve(string? providerId, bool automationProfile)
    {
        if (automationProfile) return new FakeAiAssistantProvider();
        return string.Equals(providerId, OpenAiAssistantProviderFoundation.Id, StringComparison.OrdinalIgnoreCase)
            ? new OpenAiAssistantProviderFoundation()
            : new LocalAiAssistantProvider();
    }

    public static string NormalizeProviderId(string? providerId) =>
        string.Equals(providerId, OpenAiAssistantProviderFoundation.Id, StringComparison.OrdinalIgnoreCase)
            ? OpenAiAssistantProviderFoundation.Id
            : LocalAiAssistantProvider.Id;
}
