using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Services;

public sealed class FallbackMarketDataProvider : IMarketDataProvider
{
    public string Name => "fallback";

    public Task<IReadOnlyList<CompanyEntry>> GetCompaniesAsync(string market, CancellationToken cancellationToken)
    {
        var baseTickers = market.ToUpperInvariant() switch
        {
            "US100" => ["NVDA", "AAPL", "MSFT", "AMZN", "GOOGL", "META", "TSLA", "NFLX", "AMD", "COST"],
            "US30" => ["GS", "CAT", "UNH", "MSFT", "AMGN", "AXP", "V", "HD", "TRV", "SHW"],
            "GER40" => ["SIE", "ALV", "SAP", "ENR", "DTE", "AIR", "MUV2", "RHM", "IFX", "DBK"],
            "XAUUSD" => ["NEM", "AEM", "GOLD", "WPM", "AU", "FNV", "GFI", "KGC", "LUG", "SHANDONG"],
            _ => ["SPY", "QQQ", "DIA", "IWM", "GLD"]
        };

        var weightedTotal = baseTickers.Length;
        var sequence = DateTimeOffset.UtcNow.Minute;
        var companies = baseTickers.Select((ticker, index) =>
        {
            var signal = (decimal)Math.Sin((sequence + index + 1) * 0.33);
            var percentageChange = decimal.Round(signal * 2.75m, 2);
            var price = decimal.Round(120m + (decimal)Math.Abs(Math.Cos((sequence + index + 1) * 0.52)) * 320m, 2);

            return new CompanyEntry
            {
                Ticker = ticker,
                CompanyName = ticker,
                OfficialWeight = decimal.Round(1m / weightedTotal, 4),
                PercentageChange = percentageChange,
                Price = price
            };
        }).ToList();

        return Task.FromResult<IReadOnlyList<CompanyEntry>>(companies);
    }
}
