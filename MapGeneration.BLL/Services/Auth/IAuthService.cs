using MapGeneration.BLL.Models.Users;

namespace MapGeneration.BLL.Services.Auth;

public interface IAuthService
{
    public string EncodeToken(UserModel userModel);
    public Task<bool> VerifyToken(string token, UserModel userModel);
    public bool VerifyPassword(string password, string passwordHash);
    public string HashPassword(string password);
}