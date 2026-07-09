namespace Application.Interfaces;

public interface IProviderSelector
{
    IMarketDataProvider SelectProvider(IReadOnlyList<IMarketDataProvider> providers, string market);

    IReadOnlyList<string> GetConfiguredProviderOrder();
}
