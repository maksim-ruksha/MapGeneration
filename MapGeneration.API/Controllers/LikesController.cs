using MapGeneration.BLL.Models;
using MapGeneration.BLL.Models.Users;
using MapGeneration.BLL.Services;
using MapGeneration.DAL.Entities;
using MapGeneration.DAL.Entities.Users;
using Microsoft.AspNetCore.Mvc;

namespace MapGeneration.API.Controllers;

[ApiController]
[Route("likes")]
public class LikesController: Controller
{

    private readonly ILogger<LikesController> _logger;
    private readonly IService<LikeModel, LikeEntity> _likeService;
    private readonly IService<UserModel, UserEntity> _userService;
    
    public LikesController(
        ILogger<LikesController> logger,
        IService<LikeModel, LikeEntity> likeService,
        IService<UserModel, UserEntity> userService
        )
    {
        _logger = logger;
        _likeService = likeService;
        _userService = userService;
    }
    
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