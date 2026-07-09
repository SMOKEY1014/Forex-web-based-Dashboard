using Application.Models;
using Domain.Entities;

namespace Services;

public sealed class MarketIntelligenceService
{
    private static readonly IReadOnlyDictionary<string, decimal> FactorWeights = new Dictionary<string, decimal>
    {
        ["Weighted Companies"] = 45m,
        ["Index Futures"] = 20m,
        ["Market Breadth"] = 10m,
        ["Economic Calendar"] = 10m,
        ["VIX"] = 5m,
        ["DXY"] = 5m,
        ["US10Y"] = 5m
    };

    public MarketSnapshot BuildSnapshot(
        string market,
        IReadOnlyList<CompanyEntry> companies,
        bool isExtendedHours,
        string providerUsed = "stub",
        IReadOnlyList<EconomicCalendarEvent>? economicEvents = null,
        IReadOnlyList<MarketAlert>? alerts = null)
    {
        var events = economicEvents ?? [];
        var positiveCount = companies.Count(company => company.PercentageChange > 0);
        var negativeCount = companies.Count(company => company.PercentageChange < 0);
        var neutralCount = Math.Max(0, companies.Count - positiveCount - negativeCount);

        var pressure = companies.Sum(company => company.Contribution);
        var bullishPercent = companies.Count == 0 ? 0 : decimal.Round((decimal)positiveCount / companies.Count * 100m, 2);
        var bearishPercent = companies.Count == 0 ? 0 : decimal.Round((decimal)negativeCount / companies.Count * 100m, 2);
        var neutralPercent = companies.Count == 0 ? 0 : decimal.Round((decimal)neutralCount / companies.Count * 100m, 2);

        var factorScores = BuildFactorScores(companies, pressure, bullishPercent, bearishPercent, events);
        var score = ComputeWeightedScore(factorScores);
        var bias = BuildBias(pressure, bullishPercent, bearishPercent, score, factorScores);

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
            FactorBreakdown = new Dictionary<string, decimal>(FactorWeights),
            FactorScores = factorScores,
            IsExtendedHours = isExtendedHours,
            UpdatedAtUtc = DateTimeOffset.UtcNow,
            Alerts = alerts?.ToList() ?? [],
            EconomicEventsCount = events.Count,
            ProviderUsed = providerUsed
        };
    }

    private static decimal ComputeWeightedScore(IReadOnlyDictionary<string, decimal> factors)
    {
        var score = FactorWeights.Sum(weight => factors[weight.Key] * (weight.Value / 100m));
        return decimal.Round(Math.Clamp(score, 0m, 100m), 2);
    }

    private static Dictionary<string, decimal> BuildFactorScores(
        IReadOnlyList<CompanyEntry> companies,
        decimal pressure,
        decimal bullishPercent,
        decimal bearishPercent,
        IReadOnlyList<EconomicCalendarEvent> economicEvents)
    {
        var weightedCompanies = Math.Clamp(pressure * 18m + 50m, 0m, 100m);
        var breadth = Math.Clamp(50m + (bullishPercent - bearishPercent) * 0.5m, 0m, 100m);
        var averageMove = companies.Count == 0 ? 0m : companies.Average(company => company.PercentageChange);
        var volatility = companies.Count == 0 ? 0m : companies.Average(company => Math.Abs(company.PercentageChange));

        var futures = Math.Clamp(50m + averageMove * 8m, 0m, 100m);
        var eventSentiment = economicEvents.Count == 0 ? 0m : economicEvents.Average(item => item.SentimentScore);
        var eventImpact = economicEvents.Count == 0
            ? 50m
            : Math.Clamp(50m + eventSentiment * 30m + economicEvents.Count(item => item.Impact == "high") * 3m, 0m, 100m);
        var vix = Math.Clamp(70m - volatility * 8m, 0m, 100m);
        var dxy = Math.Clamp(50m - pressure * 7m, 0m, 100m);
        var us10y = Math.Clamp(52m - eventSentiment * 15m, 0m, 100m);

        return new Dictionary<string, decimal>
        {
            ["Weighted Companies"] = decimal.Round(weightedCompanies, 2),
            ["Index Futures"] = decimal.Round(futures, 2),
            ["Market Breadth"] = decimal.Round(breadth, 2),
            ["Economic Calendar"] = decimal.Round(eventImpact, 2),
            ["VIX"] = decimal.Round(vix, 2),
            ["DXY"] = decimal.Round(dxy, 2),
            ["US10Y"] = decimal.Round(us10y, 2)
        };
    }

    private static MarketBiasResult BuildBias(
        decimal pressure,
        decimal bullishPercent,
        decimal bearishPercent,
        decimal score,
        IReadOnlyDictionary<string, decimal> factors)
    {
        var bias = score switch
        {
            >= 75m => "Strong Bullish",
            >= 60m => "Bullish",
            <= 25m => "Strong Bearish",
            <= 40m => "Bearish",
            _ => "Neutral"
        };

        var stability = decimal.Round((factors["VIX"] + factors["Economic Calendar"]) / 2m, 2);
        var confidence = decimal.Round(Math.Clamp(Math.Abs(score - 50m) * 1.9m + stability * 0.35m, 35m, 97m), 2);

        return new MarketBiasResult
        {
            MarketBiasScore = score,
            Confidence = confidence,
            BiasLabel = bias,
            Explanation = $"Weighted pressure is {pressure:F2}, breadth is {bullishPercent:F0}% bullish vs {bearishPercent:F0}% bearish.",
            FactorBreakdown = new Dictionary<string, decimal>(FactorWeights)
        };
    }
}
