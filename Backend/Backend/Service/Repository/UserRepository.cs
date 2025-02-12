using Backend.Dto;
using Backend.MongoDb.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Service.Repository;

public class UserRepository : MongoRepository<UserData>, IUserRepository
{
    private readonly IMongoCollection<UserData> _usersCollection;

    public UserRepository(IMongoDatabase database, string collectionName) : base(database, collectionName)
    {
        if (database == null)
            throw new ArgumentNullException(nameof(database));
        if (string.IsNullOrEmpty(collectionName))
            throw new ArgumentNullException(nameof(collectionName));

        _usersCollection = database.GetCollection<UserData>(collectionName);
    }

   public async Task<bool> IncrementWinsAsync(string userId, string type)
    {
        var filter = Builders<UserData>.Filter.Eq(u => u.Id, userId);
        var user = await _usersCollection.Find(filter).FirstOrDefaultAsync();
        if (user == null)
        {
           Console.WriteLine("itt a hiba");
        }
        UpdateDefinition<UserData> update;
        
        switch (type)
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