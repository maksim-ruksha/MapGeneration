using AutoMapper;
using MapGeneration.DAL.EF;
using NLog;

namespace MapGeneration.BLL.Services;

public class Service<TModel, TEntity> : IService<TModel, TEntity> where TModel : class where TEntity: class
{
    private readonly ILogger _logger;
    private readonly IRepository<TEntity> _repository;
    private readonly IMapper _mapper;

    public Service(ILogger logger)
    {
        _logger = logger;
    }
    public async Task<IEnumerable<TModel>> GetAsync(Func<TEntity, bool> filter)
    {
        return _mapper.Map<IEnumerable<TModel>>(await _repository.GetAsync(filter));
    }

    public async Task<TModel> FindAsync(Guid id)
    {
        return _mapper.Map<TModel>(await _repository.FindAsync(id));
    }

    public async Task<bool> CreateAsync(TModel item)
    {
        try
        {
            TEntity entity = _mapper.Map<TEntity>(item);
            await _repository.CreateAsync(entity);
            return true;
        }
        catch (Exception e)
        {
            // TODO: log
        }

        return false;
    }

    public async Task<bool> RemoveAsync(TModel item)
    {
        try
        {
            TEntity entity = _mapper.Map<TEntity>(item);
            await _repository.RemoveAsync(entity);
        }
        catch (Exception e)
        {
            // TODO: log
            return false;
        }

        return true;
    }

    public async Task<bool> UpdateAsync(TModel item)
    {
        try
        {
            TEntity entity = _mapper.Map<TEntity>(item);
            await _repository.UpdateAsync(entity);
        }
        catch (Exception e)
        {
            // TODO: log
            return false;
        }

        return true;
    }
}