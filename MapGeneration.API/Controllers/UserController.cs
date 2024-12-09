using MapGeneration.BLL.Models.Users;
using MapGeneration.BLL.Services;
using MapGeneration.BLL.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapGeneration.API.Controllers;

[ApiController]
[Route("users")]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public UserController(
        ILogger<UserController> logger,
        IUserService userService,
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
    public async Task<ActionResult> Register([FromQuery] RegisterModel registerModel)
    {
        bool isNamedUserExists = await _userService.ExistsByNameAsync(registerModel.Name);

        if (isNamedUserExists)
        {
            return Conflict("User with this name already exists");
        }

        if (registerModel.Password != registerModel.PasswordRepeat)
        {
            return BadRequest("Passwords are not same");
        }

        UserModel registeredUser = await _userService.RegisterAsync(registerModel);
        return Ok(registeredUser);
    }

    [HttpGet("login")]
    public async Task<ActionResult> Login([FromQuery] LoginModel login)
    {
        bool isUserExists = await _userService.ExistsByNameAsync(login.Name);
        if (isUserExists)
        {
            return NotFound("Invalid user name");
        }

        UserModel user = await _userService.FindByNameAsync(login.Name);
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
        bool isUserExists = await _userService.ExistsByNameAsync(userName);
        if (!isUserExists)
        {
            return BadRequest();
        }

        UserModel userModel = await _userService.FindByNameAsync(userName);

        bool tokenIsLegit = await _authService.VerifyToken(token, userModel);
        if (!tokenIsLegit)
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpGet("name/{userName}")]
    public async Task<ActionResult> GetByName(string userName)
    {
        UserModel user = await _userService.FindByNameAsync(userName);
        return Ok(user);
    }

    [HttpGet("id/{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        bool isUserExists = await _userService.ExistsAsync(id);
        if (!isUserExists)
        {
            return NotFound();
        }
        UserModel user = await _userService.FindAsync(id);
        return Ok(user);
    }
}