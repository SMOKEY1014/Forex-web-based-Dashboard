namespace Application.Models;

public sealed class MarketBiasResult
{
    public decimal MarketBiasScore { get; init; }

    public decimal Confidence { get; init; }

    public string BiasLabel { get; init; } = "Neutral";

    public string Explanation { get; init; } = string.Empty;

    public Dictionary<string, decimal> FactorBreakdown { get; init; } = [];
}
