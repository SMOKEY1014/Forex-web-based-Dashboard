using Application.Interfaces;
using Application.Models;
using Domain.Entities;

namespace Services;

public sealed class MarketIntelligenceService
{
    public MarketSnapshot BuildSnapshot(string market, IReadOnlyList<CompanyEntry> companies, bool isExtendedHours)
    {
        var positiveCount = companies.Count(company => company.PercentageChange > 0);
        var negativeCount = companies.Count(company => company.PercentageChange < 0);
        var neutralCount = Math.Max(0, companies.Count - positiveCount - negativeCount);

        var pressure = companies.Sum(company => company.Contribution);
        var bullishPercent = companies.Count == 0 ? 0 : decimal.Round((decimal)positiveCount / companies.Count * 100m, 2);
        var bearishPercent = companies.Count == 0 ? 0 : decimal.Round((decimal)negativeCount / companies.Count * 100m, 2);
        var neutralPercent = companies.Count == 0 ? 0 : decimal.Round((decimal)neutralCount / companies.Count * 100m, 2);

        var bias = BuildBias(pressure, bullishPercent, bearishPercent);

        return new MarketSnapshot
        {
            Market = market,
            Companies = [.. companies],
            TotalPressure = decimal.Round(pressure, 4),
            BullishPercentage = bullishPercent,
            BearishPercentage = bearishPercent,
            NeutralPercentage = neutralPercent,
            MarketBiasScore = bias.MarketBiasScore,
            Confidence = bias.Confidence,
            BiasLabel = bias.BiasLabel,
            Explanation = bias.Explanation,
            FactorBreakdown = bias.FactorBreakdown,
            IsExtendedHours = isExtendedHours,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
    }

    private static MarketBiasResult BuildBias(decimal pressure, decimal bullishPercent, decimal bearishPercent)
    {
        var weightedCompanies = Math.Clamp(pressure * 18m + 50m, 0m, 100m);
        var breadth = Math.Clamp(50m + (bullishPercent - bearishPercent) * 0.5m, 0m, 100m);
        const decimal futures = 55m;
        const decimal economicCalendar = 50m;
        const decimal vix = 50m;
        const decimal dxy = 50m;
        const decimal us10y = 50m;

        var score = decimal.Round(
            weightedCompanies * 0.45m +
            futures * 0.2m +
            breadth * 0.1m +
            economicCalendar * 0.1m +
            vix * 0.05m +
            dxy * 0.05m +
            us10y * 0.05m,
            2);

        var bias = score switch
        {
            >= 75m => "Strong Bullish",
            >= 60m => "Bullish",
            <= 25m => "Strong Bearish",
            <= 40m => "Bearish",
            _ => "Neutral"
        };

        var confidence = decimal.Round(Math.Clamp(Math.Abs(score - 50m) * 2m, 40m, 95m), 2);

        return new MarketBiasResult
        {
            MarketBiasScore = score,
            Confidence = confidence,
            BiasLabel = bias,
            Explanation = $"Weighted pressure is {pressure:F2}, breadth is {bullishPercent:F0}% bullish vs {bearishPercent:F0}% bearish.",
            FactorBreakdown = new Dictionary<string, decimal>
            {
                ["Weighted Companies"] = 45,
                ["Index Futures"] = 20,
                ["Market Breadth"] = 10,
                ["Economic Calendar"] = 10,
                ["VIX"] = 5,
                ["DXY"] = 5,
                ["US10Y"] = 5
            }
        };
    }
}
