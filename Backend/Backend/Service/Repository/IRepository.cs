namespace Backend.Service.Repository;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(string id);
    Task<List<T>> GetAllAsync();
    Task CreateAsync(T entity);
    Task UpdateAsync(string id, T entity);
    Task DeleteAsync(string id);
}