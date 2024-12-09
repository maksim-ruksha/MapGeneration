using MapGeneration.DAL.Entities.Users;

namespace MapGeneration.DAL.EF.Repositories;

public interface IUserRepository
{
    Task<UserEntity> CreateAsync(UserEntity user);
    Task<UserEntity> FindAsync(Guid id);
    Task<UserEntity> FindByNameAsync(string name);
    Task<UserEntity> UpdateAsync(UserEntity user);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> RemoveAsync(UserEntity user);
    Task<bool> RemoveByIdAsync(Guid id);
}