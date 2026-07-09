using Application.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Infrastructure.Services;

public sealed class ProviderSelector(IOptionsMonitor<ProviderOptions> providerOptions) : IProviderSelector
{
    private readonly ConcurrentDictionary<string, int> _providerIndexes = new(StringComparer.OrdinalIgnoreCase);

    public IMarketDataProvider SelectProvider(IReadOnlyList<IMarketDataProvider> providers, string market)
    {
        if (providers.Count == 0)
        {
            throw new InvalidOperationException("No market providers are registered.");
        }

        var order = BuildProviderOrder();
        var available = providers.ToDictionary(provider => provider.Name, StringComparer.OrdinalIgnoreCase);
        var configuredProviders = order.Where(providerName => available.ContainsKey(providerName)).ToList();
        if (configuredProviders.Count == 0)
        {
            return providers[0];
        }

        if (!providerOptions.CurrentValue.EnableProviderRotation || configuredProviders.Count == 1)
        {
            return available[configuredProviders[0]];
        }

        var index = _providerIndexes.AddOrUpdate(market, 0, (_, current) => (current + 1) % configuredProviders.Count);
        return available[configuredProviders[index]];
    }

    public IReadOnlyList<string> GetConfiguredProviderOrder() => BuildProviderOrder();

    private IReadOnlyList<string> BuildProviderOrder()
    {
        var options = providerOptions.CurrentValue;
        var configured = options.RotatingProviders
            .Where(providerName => !string.IsNullOrWhiteSpace(providerName))
            .Select(providerName => providerName.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (configured.Count == 0 && !string.IsNullOrWhiteSpace(options.PrimaryStocksProvider))
        {
            configured.Add(options.PrimaryStocksProvider.Trim());
        }

        return configured;
    }
}
