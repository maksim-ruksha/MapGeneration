using System.Collections;
using MapGeneration.DAL.Entities;
using MapGeneration.DAL.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace MapGeneration.DAL.EF.Repositories;

public class MapRepository : IMapRepository
{
    private readonly DbContext _context;
    private readonly DbSet<MapEntity> _dbSet;

    public MapRepository(DatabaseContext context)
    {
        _context = context;
        _dbSet = context.Set<MapEntity>();
    }

    public async Task<MapEntity> FindAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<MapEntity> UpdateAsync(MapEntity map)
    {
        _dbSet.Update(map);
        MapEntity result = await _dbSet.FindAsync(map.Id);
        return result;
    }

    public async Task<MapEntity> CreateAsync(MapEntity map)
    {
        await _dbSet.AddAsync(map);
        await _context.SaveChangesAsync();
        return await _dbSet.FindAsync(map.Id);
    }

    public IEnumerable<MapEntity> GetPagedAsync(int page, int pageSize, string sortField, bool ascending)
    {
        int toSkip = page * pageSize;
        int toTake = pageSize;
        IEnumerable<MapEntity> result;
        if (ascending)
        {
            result = _dbSet.OrderByColumn(sortField).Skip(toSkip).Take(toTake);
        }
        else
        {
            result = _dbSet.OrderByColumnDescending(sortField).Skip(toSkip).Take(toTake);
        }

        return result;
    }


    public IEnumerable<MapEntity> GetPagedByUserAsync(
        int page,
        int pageSize,
        string sortField,
        bool ascending,
        UserEntity user
    )
    {
        int toSkip = page * pageSize;
        int toTake = pageSize;
        IEnumerable<MapEntity> result;
        if (ascending)
        {
            result = _dbSet.OrderByColumn(sortField).Where(map => map.Author.Id == user.Id).Skip(toSkip).Take(toTake);
        }
        else
        {
            result = _dbSet.OrderByColumnDescending(sortField).Where(map => map.Author.Id == user.Id).Skip(toSkip)
                .Take(toTake);
        }

        return result;
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        return await _dbSet.AnyAsync(map => map.Id == id);
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        if (await ExistsByIdAsync(id))
        {
            MapEntity entity = await FindAsync(id);
            _dbSet.Remove(entity);
            return true;
        }

        return false;
    }

    public async Task<bool> RemoveAsync(MapEntity entity)
    {
        if (await ExistsByIdAsync(entity.Id))
        {
            _dbSet.Remove(entity);
            return true;
        }

        return false;
    }

    public long Count()
    {
        return _dbSet.Count();
    }
}