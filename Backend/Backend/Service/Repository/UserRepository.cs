using Backend.MongoDb.Model;
using MongoDB.Driver;

namespace Backend.Service.Repository;

public class UserRepository : MongoRepository<UserData>, IUserRepository
{
    private readonly IMongoCollection<UserData> _usersCollection;

    public UserRepository(IMongoDatabase database, string collectionName) : base(database, collectionName)
    {
    }

   public async Task IncrementWinsAsync(string userId, string type)
    {
        var filter = Builders<UserData>.Filter.Eq(u => u.Id, userId);
        UpdateDefinition<UserData> update;
        
        switch (type.ToLower())
        {
            case "stone":
                update = Builders<UserData>.Update.Inc(u => u.GamesWin.Stone, 1);
                break;
            case "scissor":
                update = Builders<UserData>.Update.Inc(u => u.GamesWin.Scissor, 1);
                break;
            case "paper":
                update = Builders<UserData>.Update.Inc(u => u.GamesWin.Paper, 1);
                break;
            default:
                return;
        }
        await _usersCollection.UpdateOneAsync(filter, update);

    }
}