using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.MongoDb.Model;

public class UserData
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    
    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("simulationStateIds")]
    public List<string> SimulationStateIds { get; set; } = new();

    [BsonElement("gamesPlayed")]
    public int GamesPlayed { get; set; }

    [BsonElement("gamesWin")]
    public GamesWin GamesWin { get; set; } = new GamesWin();
    

}