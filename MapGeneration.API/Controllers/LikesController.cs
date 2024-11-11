using Microsoft.AspNetCore.Mvc;

namespace MapGeneration.API.Controllers;

[ApiController]
[Route("likes")]
public class LikesController: Controller
{
    [HttpGet("{mapId}")]
    public Task<ActionResult> GetMapLikes(long mapId)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("add")]
    public Task<ActionResult> AddLike(long mapId)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("userLikeExists")]
    public Task<ActionResult> UserLikeExists(long userId, long mapId)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("delete")]
    public Task<ActionResult> DeleteLike(long userId, long mapId)
    {
        throw new NotImplementedException();
    }
}