namespace Infrastructure.Options;

public sealed class ProviderOptions
{
    public const string SectionName = "Providers";

    public string PrimaryStocksProvider { get; init; } = "stub";

    public bool UseExtendedHours { get; set; }

    public int RefreshIntervalSeconds { get; set; } = 30;
}
