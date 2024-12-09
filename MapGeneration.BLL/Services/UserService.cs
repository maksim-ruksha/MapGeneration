using AutoMapper;
using MapGeneration.BLL.Models.Users;
using MapGeneration.BLL.Services.Auth;
using MapGeneration.DAL.EF.Repositories;
using MapGeneration.DAL.Entities.Users;
using UserRole = MapGeneration.BLL.Models.Users.UserRole;

namespace MapGeneration.BLL.Services;

// TODO: exceptions, logging etc
public class UserService: IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IAuthService authService,
        IMapper mapper
            )
    {
        _userRepository = userRepository;
        _authService = authService;
        _mapper = mapper;
    }
    
    public async Task<UserModel> RegisterAsync(RegisterModel register)
    {
        UserModel newUser = new UserModel();
        newUser.PasswordHash = _authService.HashPassword(register.Password);
        newUser.Name = register.Name;
        newUser.Role = UserRole.Default;

        UserEntity newUserEntity = _mapper.Map<UserEntity>(newUser);
        
        UserModel createdUser = _mapper.Map<UserModel>(await _userRepository.CreateAsync(newUserEntity));
        return createdUser;
    }

    public async Task<UserModel> FindAsync(Guid id)
    {
        return _mapper.Map<UserModel>(await _userRepository.FindAsync(id));
    }

    public async Task<UserModel> FindByNameAsync(string name)
    {
        return _mapper.Map<UserModel>(await _userRepository.FindByNameAsync(name));
    }

    public bool CheckPassword(UserModel userModel, string password)
    {
        return _authService.VerifyPassword(password, userModel.PasswordHash);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _userRepository.ExistsAsync(id);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _userRepository.ExistsByNameAsync(name);
    }
}