using System.Linq.Expressions;

// 1. Define the base interface with TWO parameters
public interface IRepo<T, TId> where T : class
{
    // Change 'int id' to 'TId id' so it's truly generic
    Task<T?> GetByIdAsync(TId id);
    Task<List<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(List<T> entities);
    Task AddRangeAsync(List<T> entities);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    IQueryable<T> GetQuery();
    Task SaveAsync();
}

public interface IRepo<T> : IRepo<T, int> where T : class
{
}
