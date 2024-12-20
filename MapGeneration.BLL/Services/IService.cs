﻿using MapGeneration.BLL.Models.Users;

namespace MapGeneration.BLL.Services;

public interface IService<TModel, TEntity> where TModel: class where TEntity: class
{
    public Task<IEnumerable<TModel>> GetAsync(Func<TEntity, bool> filter);
    public Task<TModel> GetFirstAsync(Func<TEntity, bool> filter);
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
        SortingDirection direction,
        Func<TEntity, bool> filter
        );
    public Task<TModel> FindAsync(Guid id);
    public Task<TModel> FindAsNoTrackingAsync(Guid id);
    public Task<bool> CreateAsync(TModel item);
    public Task<bool> RemoveAsync(TModel item);
    public Task<bool> UpdateAsync(TModel item);
    public Task<IEnumerable<TModel>> AsNoTracking(Func<TEntity, bool> filter);
    public Task<TModel> AsNoTrackingFirst(Func<TEntity, bool> filter);
    public Task<long> Count();
}