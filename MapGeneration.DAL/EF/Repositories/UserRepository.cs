using MapGeneration.DAL.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace MapGeneration.DAL.EF.Repositories;

public class UserRepository: IUserRepository
{
    private readonly DbContext _context;
    private readonly DbSet<UserEntity> _dbSet;
    
    public UserRepository(DatabaseContext context)
    {
        _context = context;
        _dbSet = context.Set<UserEntity>();
    }
    
    public async Task<UserEntity> CreateAsync(UserEntity user)
    {
        await _dbSet.AddAsync(user);
        await _context.SaveChangesAsync();
        UserEntity createdUser = await _dbSet.FindAsync(user.Id);
        return createdUser;
    }

    public async Task<UserEntity> FindAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<UserEntity> FindByNameAsync(string name)
    {
        return await _dbSet.FirstAsync(user => user.Name == name);
    }

    public async Task<UserEntity> UpdateAsync(UserEntity user)
    {
        _dbSet.Update(user);
        UserEntity result = await _dbSet.FindAsync(user.Id);
        return result;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(user => user.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbSet.AnyAsync(user => user.Name == name);
    }

    public async Task<bool> RemoveAsync(UserEntity user)
    {
        if (await ExistsAsync(user.Id))
        {
            _dbSet.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        if (await ExistsAsync(id))
        {
            UserEntity user = await FindAsync(id);
            _dbSet.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}