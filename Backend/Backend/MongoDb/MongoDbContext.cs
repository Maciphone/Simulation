using Backend.MongoDb.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Backend.MongoDb;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly string _usersDataCollectionName;
    private readonly string _simulationStateCollectionName;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var mongoSettings = settings.Value;
        var client = new MongoClient(mongoSettings.ConnectionString);
        _database = client.GetDatabase(mongoSettings.DatabaseName);

        _usersDataCollectionName = mongoSettings.UsersDataCollectionName;
        _simulationStateCollectionName = mongoSettings.SimulationStateCollectionName;
    }

    public IMongoCollection<UserData> UsersData => _database.GetCollection<UserData>(_usersDataCollectionName);
    public IMongoCollection<SimulationState> SimulationStates => _database.GetCollection<SimulationState>(_simulationStateCollectionName);
    

}