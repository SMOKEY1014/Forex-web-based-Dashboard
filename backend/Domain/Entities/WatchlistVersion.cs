using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public sealed class WatchlistVersion
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = string.Empty;

    public required string Market { get; init; }

    public int Version { get; init; }

    public string Operation { get; init; } = "upsert";

    public Watchlist? Snapshot { get; init; }

    public DateTimeOffset ChangedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
