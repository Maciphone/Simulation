using Backend.MongoDb.Model;

namespace Backend.Service.Repository;

public interface ISimulationStateRepository  :IRepository<SimulationState>
{
    Task<SimulationState> GetBySimulationIdAsync(string simulationId);
}