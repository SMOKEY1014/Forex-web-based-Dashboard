namespace Infrastructure.Options;

public sealed class ProviderOptions
{
    public const string SectionName = "Providers";

    public string PrimaryStocksProvider { get; init; } = "stub";

    public List<string> RotatingProviders { get; init; } = ["stub", "fallback"];

    public bool EnableProviderRotation { get; set; } = true;

    public bool UseExtendedHours { get; set; }

    public int RefreshIntervalSeconds { get; set; } = 30;
}
