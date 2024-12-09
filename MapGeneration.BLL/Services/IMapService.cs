using MapGeneration.BLL.Models;
using MapGeneration.BLL.Models.Users;

namespace MapGeneration.BLL.Services;

public interface IMapService
{
    Task<MapModel> Create(MapModel mapModel);
    Task<MapModel> FindAsync(Guid id);
    Task<MapModel> UpdateAsync(MapModel mapModel);
    Task<IEnumerable<MapModel>> GetPagedAsync(int page, int pageSize, string sortField, SortingDirection direction);
    Task<IEnumerable<MapModel>> GetPagedByUserAsync(int page, int pageSize, string sortField, SortingDirection direction, UserModel userModel);
    int GetPagesCount(int pageSize);

}