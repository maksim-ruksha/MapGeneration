namespace MapGeneration.DAL.EF;

public interface IRepository<T> where T: class 
{
    public Task<T> CreateAsync(T item);
    public Task<T> FindAsync(Guid id);
    public Task<IEnumerable<T>> GetAsync(Func<T, bool> filter);
    public Task RemoveAsync(T item);
    public Task UpdateAsync(T item);
}