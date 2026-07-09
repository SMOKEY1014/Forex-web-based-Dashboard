using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Services;

public sealed class StubMarketDataProvider : IMarketDataProvider
{
    private static readonly Dictionary<string, string[]> MarketTickers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["US100"] = ["NVDA", "AAPL", "MSFT", "AMZN", "GOOGL", "GOOG", "AVGO", "META", "TSLA", "NFLX"],
        ["US30"] = ["GS", "CAT", "UNH", "MSFT", "GOOGL", "AMGN", "AXP", "V", "HD", "TRV", "SHW"],
        ["GER40"] = ["SIE", "ALV", "SAP", "ENR", "DTE", "AIR", "MUV2", "RHM", "IFX", "DBK"],
        ["XAUUSD"] = ["NEM", "AEM", "GOLD", "WPM", "AU", "FNV", "GFI", "KGC", "LUG", "SHANDONG"]
    };

    public string Name => "stub";

    public Task<IReadOnlyList<CompanyEntry>> GetCompaniesAsync(string market, CancellationToken cancellationToken)
    {
        var tickers = MarketTickers.TryGetValue(market, out var configured) ? configured : ["SPY"];
        var random = Random.Shared;
        var total = tickers.Length;
        var companies = tickers.Select((ticker, index) =>
        {
            var weight = Math.Round((decimal)(1.0 / total), 4);
            var percentageChange = Math.Round((decimal)(random.NextDouble() * 6 - 3), 2);
            var price = Math.Round((decimal)(100 + random.NextDouble() * 400), 2);

            return new CompanyEntry
            {
                Ticker = ticker,
                CompanyName = ticker,
                OfficialWeight = weight,
                PercentageChange = percentageChange,
                Price = price
            };
        }).ToList();

        return Task.FromResult<IReadOnlyList<CompanyEntry>>(companies);
    }
}
