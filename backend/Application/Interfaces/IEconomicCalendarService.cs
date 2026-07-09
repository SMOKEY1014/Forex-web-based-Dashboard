using Domain.Entities;

namespace Application.Interfaces;

public interface IEconomicCalendarService
{
    Task<IReadOnlyList<EconomicCalendarEvent>> GetUpcomingAsync(string market, DateTimeOffset fromUtc, DateTimeOffset toUtc, CancellationToken cancellationToken);

    Task IngestLatestAsync(CancellationToken cancellationToken);
}
