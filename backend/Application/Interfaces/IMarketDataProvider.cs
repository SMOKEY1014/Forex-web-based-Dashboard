using Domain.Entities;

namespace Application.Interfaces;

public interface IMarketDataProvider
{
    string Name { get; }

    Task<IReadOnlyList<CompanyEntry>> GetCompaniesAsync(string market, CancellationToken cancellationToken);
}
