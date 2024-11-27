using MapGeneration.API.Auth;
using MapGeneration.BLL.Models.Users;
using MapGeneration.BLL.Services;
using MapGeneration.DAL.Entities.Users;
using Microsoft.AspNetCore.Mvc;
using UserRole = MapGeneration.BLL.Models.Users.UserRole;

namespace MapGeneration.API.Controllers;

[ApiController]
[Route("user")]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IService<UserModel, UserEntity> _userService;

    public UserController(
        ILogger<UserController> logger,
        IService<UserModel, UserEntity> userService
    )
    {
        _logger = logger;
        _userService = userService;
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

        UserModel newUser = new UserModel
        {
            Name = register.Name,
            Role = UserRole.Default,
            PasswordHash = Cryptography.HashPassword(register.Password)
        };
        bool success = await _userService.CreateAsync(newUser);

        if (!success)
        {
            // TODO: replace with action result
            return StatusCode(500);
        }

        return Ok();
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
        if (!Cryptography.VerifyPassword(user.PasswordHash, login.Password))
        {
            return BadRequest("Wrong password");
        }

        return Ok();
    }

    [HttpGet("validate")]
    public Task<ActionResult> ValidateToken(string token, string userName)
    {
        throw new NotImplementedException();
    }

    [HttpGet("name/{name}")]
    public Task<ActionResult> ReadByName(string name)
    {
        throw new NotImplementedException();
    }

    [HttpGet("id/{id}")]
    public Task<ActionResult> Read(long id)
    {
        throw new NotImplementedException();
    }
}