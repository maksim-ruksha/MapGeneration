using Microsoft.AspNetCore.Mvc;

namespace MapGeneration.API.Controllers;

[ApiController]
[Route("comments")]
public class CommentsController: Controller
{
    
    [HttpGet("getByMap")]
    public async Task<ActionResult> GetMapComments(string sortField, int page, string direction, int size)
    {
        throw new NotImplementedException();
    }

    [HttpGet("getPagesCount")]
    public async Task<ActionResult> GetPagesCount(long mapId, long pageSize)
    {
        throw new NotImplementedException();
    }

    [HttpPost("send")]
    public async Task<ActionResult> SendComment( /*TODO: CommentDto*/)
    {
        throw new NotImplementedException();
    }

    [HttpGet("userCommentExists")]
    public async Task<ActionResult> IsUserCommentExists(long mapId, long userId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("getUserComment")]
    public async Task<ActionResult> GetUserComment(long mapId, long userId)
    {
        throw new NotImplementedException();
    }
}