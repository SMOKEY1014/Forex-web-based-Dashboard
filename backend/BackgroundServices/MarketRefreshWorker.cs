using Application.Interfaces;
using Infrastructure.Options;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Services;
using SignalR;

namespace BackgroundServices;

public sealed class MarketRefreshWorker(
    ILogger<MarketRefreshWorker> logger,
    IProviderSelector providerSelector,
    IEnumerable<IMarketDataProvider> providers,
    IMarketSnapshotRepository snapshotRepository,
    WatchlistService watchlistService,
    MarketIntelligenceService marketIntelligenceService,
    AlertService alertService,
    IEconomicCalendarService economicCalendarService,
    IHubContext<MarketHub> marketHub,
    IOptionsMonitor<ProviderOptions> providerOptions) : BackgroundService
{
    private readonly IReadOnlyList<IMarketDataProvider> _providers = providers.ToList();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Market refresh worker started with providers {Providers}",
            string.Join(", ", providerSelector.GetConfiguredProviderOrder()));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var options = providerOptions.CurrentValue;
                await economicCalendarService.IngestLatestAsync(stoppingToken);
                var watchlists = await watchlistService.GetOrSeedWatchlistsAsync(stoppingToken);

                foreach (var watchlist in watchlists.Where(watchlist => watchlist.AutoUpdateEnabled))
                {
                    var provider = providerSelector.SelectProvider(_providers, watchlist.Market);
                    var companies = await provider.GetCompaniesAsync(watchlist.Market, stoppingToken);
                    var economicEvents = await economicCalendarService.GetUpcomingAsync(
                        watchlist.Market,
                        DateTimeOffset.UtcNow,
                        DateTimeOffset.UtcNow.AddHours(6),
                        stoppingToken);

                    var preAlertSnapshot = marketIntelligenceService.BuildSnapshot(
                        watchlist.Market,
                        companies,
                        options.UseExtendedHours,
                        provider.Name,
                        economicEvents);
                    var alerts = alertService.Evaluate(preAlertSnapshot);
                    var snapshot = marketIntelligenceService.BuildSnapshot(
                        watchlist.Market,
                        companies,
                        options.UseExtendedHours,
                        provider.Name,
                        economicEvents,
                        alerts);

                    await snapshotRepository.SaveAsync(snapshot, stoppingToken);
                    await marketHub.Clients.All.SendAsync("market.snapshot", snapshot, stoppingToken);

                    foreach (var alert in alerts)
                    {
                        await marketHub.Clients.All.SendAsync("market.alert", alert, stoppingToken);
                    }
                }

                var delay = TimeSpan.FromSeconds(Math.Clamp(options.RefreshIntervalSeconds, 10, 300));
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error while refreshing market snapshots");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
