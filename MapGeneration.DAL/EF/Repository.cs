using Microsoft.EntityFrameworkCore;

namespace MapGeneration.DAL.EF;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DatabaseContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> CreateAsync(T item)
    {
        _dbSet.Add(item);
        await _context.SaveChangesAsync();
        IEnumerable<T> dbItemEnumerable = await GetAsync(dbItem => dbItem.Equals(item));
        return dbItemEnumerable.FirstOrDefault();
    }

    public async Task<T> FindAsync(Guid id)
    {
        T item = await _dbSet.FindAsync(id);
        return item;
    }

    public async Task<IEnumerable<T>> GetAsync(Func<T, bool> filter)
    {
        return await Task.Run(() => _dbSet.Where(filter));
    }

    public async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, string sortField, Func<T, bool> filter)
    {
        int toSkip = page * pageSize;
        return _dbSet.OrderByColumn(sortField).Skip(toSkip).Take(pageSize);;
    }

    public async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, string sortField)
    {
        int toSkip = page * pageSize;
        return _dbSet.OrderByColumn(sortField).Skip(toSkip).Take(pageSize);
    }

    public async Task RemoveAsync(T item)
    {
        _dbSet.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T item)
    {
        _context.Entry(item).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public Task<long> Count()
    {
        return Task.FromResult<long>(_dbSet.Count());
    }
}