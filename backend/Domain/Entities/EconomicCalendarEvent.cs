using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public sealed class EconomicCalendarEvent
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = string.Empty;

    public string EventCode { get; init; } = string.Empty;

    public string Market { get; init; } = string.Empty;

    public string Region { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Impact { get; init; } = "medium";

    public DateTimeOffset EventTimeUtc { get; init; }

    public decimal Forecast { get; init; }

    public decimal Previous { get; init; }

    public decimal Actual { get; init; }

    public decimal SentimentScore { get; init; }
}
