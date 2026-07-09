namespace Domain.Entities;

public sealed class CompanyEntry
{
    public required string Ticker { get; init; }

    public required string CompanyName { get; init; }

    public decimal OfficialWeight { get; init; }

    public decimal Price { get; init; }

    public decimal PercentageChange { get; init; }

    public decimal Contribution => OfficialWeight * PercentageChange;

    public bool IsLocked { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
