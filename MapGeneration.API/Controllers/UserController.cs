using Microsoft.AspNetCore.Mvc;

namespace MapGeneration.API.Controllers;

[ApiController]
[Route("user")]
public class UserController: Controller
{

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