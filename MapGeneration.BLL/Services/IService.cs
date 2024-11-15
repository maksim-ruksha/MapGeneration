﻿namespace MapGeneration.BLL.Services;

public interface IService<TModel, TEntity> where TModel: class where TEntity: class
{
    public Task<IEnumerable<TModel>> GetAsync(Func<TEntity, bool> predicate);
    public Task<TModel> FindAsync(Guid id);
    public Task<bool> CreateAsync(TModel item);
    public bool RemoveAsync(TModel item);
    public Task<bool> UpdateAsync(TModel item);
}