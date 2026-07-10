using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public sealed class Watchlist
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

    public required string Name { get; init; }

    public required string Market { get; init; }

    public List<CompanyEntry> Companies { get; init; } = [];

    public int MaximumCompanies { get; init; } = 10;

    public string Notes { get; init; } = string.Empty;

    public bool AutoUpdateEnabled { get; set; } = true;

    public bool AutoWeightRefreshEnabled { get; set; } = true;

    public int RefreshIntervalSeconds { get; set; } = 30;

    public DateTimeOffset LastUpdatedUtc { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset CreatedUtc { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset ModifiedUtc { get; set; } = DateTimeOffset.UtcNow;
}
