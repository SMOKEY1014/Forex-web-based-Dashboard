using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public sealed class MarketSnapshot
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = string.Empty;

    public required string Market { get; init; }

    public List<CompanyEntry> Companies { get; init; } = [];

    public decimal TotalPressure { get; init; }

    public decimal BullishPercentage { get; init; }

    public decimal BearishPercentage { get; init; }

    public decimal NeutralPercentage { get; init; }

    public decimal MarketBiasScore { get; init; }

    public decimal Confidence { get; init; }

    public string BiasLabel { get; init; } = "Neutral";

    public string Explanation { get; init; } = string.Empty;

    public Dictionary<string, decimal> FactorBreakdown { get; init; } = [];

    public Dictionary<string, decimal> FactorScores { get; init; } = [];

    public List<MarketAlert> Alerts { get; init; } = [];

    public int EconomicEventsCount { get; init; }

    public DateTimeOffset UpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    public bool IsExtendedHours { get; init; }

    public string ProviderUsed { get; init; } = "stub";
}
