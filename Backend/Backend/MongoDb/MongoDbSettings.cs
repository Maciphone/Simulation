namespace Backend.MongoDb;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;

    public string UsersDataCollectionName { get; set; } = null!;
    public string SimulationStateCollectionName { get; set; } = null!;
}