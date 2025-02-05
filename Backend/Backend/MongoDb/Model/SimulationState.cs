using Microsoft.AspNetCore.Http.Features;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.MongoDb.Model;

public class SimulationState
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("simulationId")]
    public string SimulationId { get; set; } = null!;

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    

    [BsonElement("items")]
    public string ItemString { get; set; }
    
    
}