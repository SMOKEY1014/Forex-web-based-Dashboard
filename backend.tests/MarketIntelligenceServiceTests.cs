using Domain.Entities;
using Services;

namespace backend.tests;

public sealed class MarketIntelligenceServiceTests
{
    [Fact]
    public void BuildSnapshot_CalculatesContributionAndPressure()
    {
        var service = new MarketIntelligenceService();
        var companies = new[]
        {
            new CompanyEntry { Ticker = "NVDA", CompanyName = "NVIDIA", OfficialWeight = 0.12m, PercentageChange = 5m, Price = 100m },
            new CompanyEntry { Ticker = "AAPL", CompanyName = "Apple", OfficialWeight = 0.08m, PercentageChange = -2m, Price = 90m }
        };

        var snapshot = service.BuildSnapshot("US100", companies, false);

        Assert.Equal(0.44m, snapshot.TotalPressure);
        Assert.Equal(50m, snapshot.BullishPercentage);
        Assert.Equal(50m, snapshot.BearishPercentage);
        Assert.Equal(0m, snapshot.NeutralPercentage);
        Assert.Contains("Weighted Companies", snapshot.FactorBreakdown.Keys);
    }
}
