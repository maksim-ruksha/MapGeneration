using MapGeneration.DAL.Entities;
using MapGeneration.DAL.Entities.Users;

namespace MapGeneration.DAL.EF.Repositories;

public interface IMapRepository
{
    Task<MapEntity> FindAsync(Guid id);
    Task<MapEntity> UpdateAsync(MapEntity map);
    Task<MapEntity> CreateAsync(MapEntity map);

    IEnumerable<MapEntity> GetPagedAsync(
        int page,
        int pageSize,
        string sortField,
        bool ascending
    );

    IEnumerable<MapEntity> GetPagedByUserAsync(
        int page,
        int pageSize,
        string sortField,
        bool ascending,
        UserEntity user
    );

    Task<bool> ExistsByIdAsync(Guid id);
    Task<bool> RemoveByIdAsync(Guid id);
    Task<bool> RemoveAsync(MapEntity entity);
    long Count();
}