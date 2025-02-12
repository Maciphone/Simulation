using Backend.Dto;
using Backend.MongoDb.Model;

namespace Backend.Service.Repository;

public interface IUserRepository : IRepository<UserData>
{
    Task<bool> IncrementWinsAsync(string userId, string type);
    Task<bool> AddSimulationStateIdAsync(string userId, string simulationStateId);
}