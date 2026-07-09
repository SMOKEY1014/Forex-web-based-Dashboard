using Domain.Entities;

namespace Services;

public sealed class AlertService
{
    public IReadOnlyList<MarketAlert> Evaluate(MarketSnapshot snapshot)
    {
        var alerts = new List<MarketAlert>();
        if (snapshot.MarketBiasScore >= 75m)
        {
            alerts.Add(CreateAlert(snapshot, "high", "bias.breakout", "Strong bullish breakout detected."));
        }

        if (snapshot.MarketBiasScore <= 25m)
        {
            alerts.Add(CreateAlert(snapshot, "high", "bias.breakdown", "Strong bearish breakdown detected."));
        }

        if (snapshot.Confidence >= 85m)
        {
            alerts.Add(CreateAlert(snapshot, "medium", "confidence.spike", "Model confidence is above 85%."));
        }

        if (Math.Abs(snapshot.TotalPressure) >= 1m)
        {
            alerts.Add(CreateAlert(snapshot, "medium", "pressure.extreme", "Total pressure reached an extreme threshold."));
        }

        return alerts;
    }

    private static MarketAlert CreateAlert(MarketSnapshot snapshot, string severity, string type, string message)
    {
        return new MarketAlert
        {
            Market = snapshot.Market,
            Severity = severity,
            Type = type,
            Message = message,
            BiasScore = snapshot.MarketBiasScore,
            Confidence = snapshot.Confidence,
            TriggeredAtUtc = DateTimeOffset.UtcNow
        };
    }
}
