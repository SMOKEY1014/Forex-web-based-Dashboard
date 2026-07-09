using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Services;

public sealed class InMemoryEconomicCalendarService : IEconomicCalendarService
{
    private readonly object _gate = new();
    private List<EconomicCalendarEvent> _events = BuildSeedEvents();

    public Task IngestLatestAsync(CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            var now = DateTimeOffset.UtcNow;
            var random = Random.Shared;
            _events = _events.Select(existing => new EconomicCalendarEvent
            {
                Id = existing.Id,
                EventCode = existing.EventCode,
                Market = existing.Market,
                Region = existing.Region,
                Title = existing.Title,
                Impact = existing.Impact,
                EventTimeUtc = now.AddMinutes(random.Next(10, 180)),
                Forecast = decimal.Round(existing.Forecast + (decimal)(random.NextDouble() - 0.5d) * 0.2m, 2),
                Actual = decimal.Round(existing.Actual + (decimal)(random.NextDouble() - 0.5d) * 0.25m, 2),
                Previous = decimal.Round(existing.Previous + (decimal)(random.NextDouble() - 0.5d) * 0.2m, 2),
                SentimentScore = decimal.Round(Math.Clamp(existing.SentimentScore + (decimal)(random.NextDouble() - 0.5d) * 0.2m, -1m, 1m), 2)
            }).ToList();
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<EconomicCalendarEvent>> GetUpcomingAsync(string market, DateTimeOffset fromUtc, DateTimeOffset toUtc, CancellationToken cancellationToken)
    {
        IReadOnlyList<EconomicCalendarEvent> events;
        lock (_gate)
        {
            events = _events
                .Where(x => string.Equals(x.Market, market, StringComparison.OrdinalIgnoreCase))
                .Where(x => x.EventTimeUtc >= fromUtc && x.EventTimeUtc <= toUtc)
                .OrderBy(x => x.EventTimeUtc)
                .ToList();
        }

        return Task.FromResult(events);
    }

    private static List<EconomicCalendarEvent> BuildSeedEvents()
    {
        var now = DateTimeOffset.UtcNow;
        return
        [
            new EconomicCalendarEvent
            {
                EventCode = "US-CPI",
                Market = "US100",
                Region = "US",
                Title = "US CPI",
                Impact = "high",
                EventTimeUtc = now.AddMinutes(45),
                Forecast = 2.9m,
                Previous = 3.1m,
                Actual = 2.8m,
                SentimentScore = 0.35m
            },
            new EconomicCalendarEvent
            {
                EventCode = "US-NFP",
                Market = "US30",
                Region = "US",
                Title = "Non-Farm Payrolls",
                Impact = "high",
                EventTimeUtc = now.AddMinutes(90),
                Forecast = 210m,
                Previous = 175m,
                Actual = 198m,
                SentimentScore = 0.2m
            },
            new EconomicCalendarEvent
            {
                EventCode = "DE-IFO",
                Market = "GER40",
                Region = "DE",
                Title = "IFO Business Climate",
                Impact = "medium",
                EventTimeUtc = now.AddMinutes(70),
                Forecast = 89.7m,
                Previous = 88.2m,
                Actual = 90.3m,
                SentimentScore = 0.45m
            },
            new EconomicCalendarEvent
            {
                EventCode = "US-REALYIELD",
                Market = "XAUUSD",
                Region = "US",
                Title = "US 10Y Real Yield",
                Impact = "high",
                EventTimeUtc = now.AddMinutes(55),
                Forecast = 1.5m,
                Previous = 1.4m,
                Actual = 1.45m,
                SentimentScore = -0.25m
            }
        ];
    }
}
