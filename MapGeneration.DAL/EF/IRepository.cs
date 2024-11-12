namespace MapGeneration.DAL.EF;

public interface IRepository<T> where T: class 
{
    public Task<T> CreateAsync(T item);
    public Task<T> FindAsync(Guid id);
    public Task<IEnumerable<T>> GetAllAsync(Func<T, bool> filter);
    public void Remove(T item);
    public Task Update(T item);
}