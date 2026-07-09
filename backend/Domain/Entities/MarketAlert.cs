namespace Domain.Entities;

public sealed class MarketAlert
{
    public string Market { get; init; } = string.Empty;

    public string Severity { get; init; } = "info";

    public string Type { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public decimal BiasScore { get; init; }

    public decimal Confidence { get; init; }

    public DateTimeOffset TriggeredAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
