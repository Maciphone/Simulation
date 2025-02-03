using MongoDB.Bson.Serialization.Attributes;

namespace Backend.MongoDb;

public class GamesWin
{

    public int Stone { get; set; }

  
    public int Scissor { get; set; }

    
    public int Paper { get; set; }
}