using Backend.Dto;
using Backend.MongoDb.Model;
using MongoDB.Driver;

namespace Backend.Service.Repository;

public class UserRepository : MongoRepository<UserData>, IUserRepository
{
    private readonly IMongoCollection<UserData> _usersCollection;

    public UserRepository(IMongoDatabase database, string collectionName) : base(database, collectionName)
    {
    }

   public async Task<bool> IncrementWinsAsync(string userId, ItemType type)
    {
        var filter = Builders<UserData>.Filter.Eq(u => u.Id, userId);
        UpdateDefinition<UserData> update;
        
        switch (type)
        {
            case ItemType.Stone:
                update = Builders<UserData>.Update.Inc(u => u.GamesWin.Stone, 1);
                break;
            case ItemType.Scissor:
                update = Builders<UserData>.Update.Inc(u => u.GamesWin.Scissor, 1);
                break;
            case ItemType.Paper:
                update = Builders<UserData>.Update.Inc(u => u.GamesWin.Paper, 1);
                break;
            default:
                return false;
        }
        var result = await _usersCollection.UpdateOneAsync(filter, update);
        return result.MatchedCount > 0;

    }
   
    public async Task<bool> AddSimulationStateIdAsync(string userId, string simulationStateId)
    {
        var filter = Builders<UserData>.Filter.Eq(u => u.Id, userId);
        var update = Builders<UserData>.Update.Push(u => u.SimulationStateIds, simulationStateId);

        var result = await _usersCollection.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }
}