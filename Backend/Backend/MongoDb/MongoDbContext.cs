using Backend.MongoDb.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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
        
        var clientSettings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
       
        var client = new MongoClient(clientSettings);
        _database = client.GetDatabase(mongoSettings.DatabaseName);

        // 📌 Most már léteznek ezek a változók
        _usersDataCollectionName = mongoSettings.UsersDataCollectionName;
        _simulationStateCollectionName = mongoSettings.SimulationStateCollectionName;
    }

    public IMongoDatabase Database => _database;
    public IMongoCollection<UserData> UsersData => _database.GetCollection<UserData>(_usersDataCollectionName);
    public IMongoCollection<SimulationState> SimulationStates => _database.GetCollection<SimulationState>(_simulationStateCollectionName);
    public IMongoCollection<ApplicationUser> Users => _database.GetCollection<ApplicationUser>("Users");
    public IMongoCollection<IdentityRole> Roles => _database.GetCollection<IdentityRole>("Roles");
}