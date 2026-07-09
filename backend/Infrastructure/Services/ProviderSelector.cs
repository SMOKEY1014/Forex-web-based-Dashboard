using Application.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public sealed class ProviderSelector(IOptionsMonitor<ProviderOptions> providerOptions) : IProviderSelector
{
    public string GetPrimaryProvider() => providerOptions.CurrentValue.PrimaryStocksProvider;
}
