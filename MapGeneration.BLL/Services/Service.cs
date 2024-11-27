using AutoMapper;
using MapGeneration.DAL.EF;
using Microsoft.Extensions.Logging;

namespace MapGeneration.BLL.Services;

public class Service<TModel, TEntity> : IService<TModel, TEntity> where TModel : class where TEntity : class
{
    private readonly IRepository<TEntity> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<Service<TModel, TEntity>> _logger;

    public Service(
        IRepository<TEntity> repository,
        IMapper mapper,
        ILogger<Service<TModel, TEntity>> logger
        )
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<IEnumerable<TModel>> GetAsync(Func<TEntity, bool> filter)
    {
        return _mapper.Map<IEnumerable<TModel>>(await _repository.GetAsync(filter));
    }

    public async Task<IEnumerable<TModel>> GetPagedAsync(
        int page,
        int pageSize,
        string sortField,
        SortingDirection direction = SortingDirection.Ascending
        )
    {
        return _mapper.Map<IEnumerable<TModel>>(await _repository.GetPagedAsync(page, pageSize, sortField));
    }

    public async Task<IEnumerable<TModel>> GetPagedAsync(
        int page,
        int pageSize,
        string sortField,
        Func<TEntity, bool> filter, SortingDirection direction = SortingDirection.Ascending
        )
    {
        return _mapper.Map<IEnumerable<TModel>>(await _repository.GetPagedAsync(page, pageSize, sortField, filter));
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
            _logger.LogError(e.Message);
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
            _logger.LogError(e.Message);
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
            _logger.LogError(e.Message);
            return false;
        }

        return true;
    }
}