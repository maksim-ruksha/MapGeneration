using System.IdentityModel.Tokens.Jwt;
using MapGeneration.BLL.Models.Users;
using MapGeneration.BLL.Services;
using MapGeneration.BLL.Services.Auth;
using MapGeneration.DAL.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserRole = MapGeneration.BLL.Models.Users.UserRole;

namespace MapGeneration.API.Controllers;

[ApiController]
[Route("users")]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IService<UserModel, UserEntity> _userService;
    private readonly IAuthService _authService;

    public UserController(
        ILogger<UserController> logger,
        IService<UserModel, UserEntity> userService,
        IAuthService authService
    )
    {
        _logger = logger;
        _userService = userService;
        _authService = authService;
    }

    [Authorize]
    [HttpGet("auth_test")]
    public ActionResult TestAuth()
    {
        return Ok();
    }

    [HttpGet("register")]
    public async Task<ActionResult> Register([FromQuery] RegisterModel register)
    {
        IEnumerable<UserModel> possibleExistingUser = await _userService.GetAsync(user => user.Name == register.Name);

        if (possibleExistingUser.Count() > 0)
        {
            return Conflict("User with this name already exists");
        }

        if (register.Password != register.PasswordRepeat)
        {
            return BadRequest("Passwords are not same");
        }

        UserModel newUser = new UserModel();
        newUser.PasswordHash = _authService.HashPassword(register.Password);
        newUser.Name = register.Name;
        newUser.Role = UserRole.Default;

        bool success = await _userService.CreateAsync(newUser);

        if (!success)
        {
            // TODO: replace with action result
            return StatusCode(500);
        }

        UserModel createdUser = await _userService.GetFirstAsync(user => user.Name == newUser.Name);

        return Ok(createdUser);
    }

    [HttpGet("login")]
    public async Task<ActionResult> Login([FromQuery] LoginModel login)
    {
        IEnumerable<UserModel> userEnumerable = await _userService.GetAsync(user => user.Name == login.Name);
        if (userEnumerable.Count() <= 0)
        {
            return NotFound("Invalid user name");
        }

        UserModel user = userEnumerable.First();
        if (!_authService.VerifyPassword(login.Password, user.PasswordHash))
        {
            return BadRequest("Wrong password");
        }

        string token = _authService.EncodeToken(user);
        return Ok(token);
    }

    [HttpGet("validate")]
    public async Task<ActionResult> ValidateToken(string token, string userName)
    {
        UserModel userModel;
        try
        {
            userModel = await _userService.GetFirstAsync(user => user.Name == userName);
        }
        catch (Exception e)
        {
            return BadRequest();
        }
        
        bool success = await _authService.VerifyToken(token, userModel);
        if (!success)
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult> GetByName(string name)
    {
        UserModel user = await _userService.GetFirstAsync(user => user.Name == name);
        return Ok(user);
    }

    [HttpGet("id/{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        UserModel user = await _userService.FindAsync(id);
        return Ok(user);
    }
}