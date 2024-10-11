using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api;

public class SensorData{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("SensorId")]
    public int SensorId { get; set; }

    public float Value { get; set; }

    public string Unit { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}