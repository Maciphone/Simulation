using Backend.MongoDb.Model;

namespace Backend.Service.Repository;

public interface IUserRepository : IRepository<UserData>
{
    Task IncrementWinsAsync(string userId, string type);
}