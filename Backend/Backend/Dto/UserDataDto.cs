using Backend.MongoDb;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Controller;

public class UserDataDto
{
    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("simulationStateIds")]
    public List<string> SimulationStateIds { get; set; } = new();

    [BsonElement("gamesPlayed")]
    public int GamesPlayed { get; set; }

    [BsonElement("gamesWin")]
    public GamesWin GamesWin { get; set; } = new GamesWin();
}