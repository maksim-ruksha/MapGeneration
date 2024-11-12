using MapGeneration.DAL.EF;

namespace MapGeneration.BLL.Services;

public class Service<TModel, TEntity> : IService<TModel, TEntity> where TModel : class where TEntity: class
{
    private readonly IRepository<TEntity> _repository;
    
    public Task<IEnumerable<TModel>> GetAsync(Func<TEntity, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<TModel> FindAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreateAsync(TModel item)
    {
        throw new NotImplementedException();
    }

    public bool RemoveAsync(TModel item)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(TModel item)
    {
        throw new NotImplementedException();
    }
}