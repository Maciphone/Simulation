using Backend.MongoDb.Model;
using MongoDB.Driver;

namespace Backend.Service.Repository;

public class SimulationStateRepository : MongoRepository<SimulationState>, ISimulationStateRepository
{
    private readonly IMongoCollection<SimulationState> _simulationStateCollection;

    public SimulationStateRepository(IMongoDatabase database, string collectionName, IMongoCollection<SimulationState> simulationStateCollection) : base(database, collectionName)
    {
        _simulationStateCollection = simulationStateCollection;
    }

    public async Task<SimulationState> GetBySimulationIdAsync(string simulationId)
    {
        var filter = Builders<SimulationState>.Filter.Eq("_id", simulationId);
        var result = await _simulationStateCollection.Find(filter).FirstOrDefaultAsync();
        return result;

    }
}