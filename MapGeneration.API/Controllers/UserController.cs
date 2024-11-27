using MapGeneration.BLL.Models.Users;
using MapGeneration.BLL.Services;
using MapGeneration.DAL.Entities.Users;
using Microsoft.AspNetCore.Mvc;

namespace MapGeneration.API.Controllers;

[ApiController]
[Route("user")]
public class UserController: Controller
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
    public Task<ActionResult> Register( /*register dto*/)
    {
        throw new NotImplementedException();
    }

    [HttpGet("login")]
    public Task<ActionResult> Login( /*login dto*/)
    {
        throw new NotImplementedException();
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