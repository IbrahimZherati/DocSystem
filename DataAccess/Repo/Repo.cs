

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class Repo<T, TId> : IRepo<T, TId> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repo(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(TId id) => await _dbSet.FindAsync(id);

    public async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Remove(T entity) => _dbSet.Remove(entity);

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task SaveAsync() => await _context.SaveChangesAsync();

    public IQueryable<T> GetQuery() => _dbSet.AsQueryable();

    public void RemoveRange(List<T> entities) => _dbSet.RemoveRange(entities);

    public async Task AddRangeAsync(List<T> entities) => await _dbSet.AddRangeAsync(entities);
}

public class Repo<T> : Repo<T, int>, IRepo<T> where T : class
{
    public Repo(AppDbContext context) : base(context)
    {
    }
}
