using MapGeneration.BLL.Models.Users;

namespace MapGeneration.BLL.Services;

public interface IUserService
{
    public Task<UserModel> RegisterAsync(RegisterModel registerModel);
    public Task<UserModel> FindAsync(Guid id);
    public Task<UserModel> FindByNameAsync(string name);
    public bool CheckPassword(UserModel userModel, string password);
    public Task<bool> ExistsAsync(Guid id);
    public Task<bool> ExistsByNameAsync(string name);
}