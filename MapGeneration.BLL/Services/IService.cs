namespace MapGeneration.BLL.Services;

public interface IService<TModel, TEntity> where TModel: class where TEntity: class
{
    public Task<IEnumerable<TModel>> GetAsync(Func<TEntity, bool> filter);
    public Task<IEnumerable<TModel>> GetPagedAsync(
        int page,
        int pageSize,
        string sortField,
        SortingDirection direction
        );
    public Task<IEnumerable<TModel>> GetPagedAsync(
        int page,
        int pageSize,
        string sortField,
        Func<TEntity, bool> filter,
        SortingDirection direction
        );
    public Task<TModel> FindAsync(Guid id);
    public Task<bool> CreateAsync(TModel item);
    public Task<bool> RemoveAsync(TModel item);
    public Task<bool> UpdateAsync(TModel item);
}