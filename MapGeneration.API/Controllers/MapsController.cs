using System.Drawing;
using System.Drawing.Imaging;
using MapGeneration.BLL.Models;
using MapGeneration.BLL.Models.Users;
using MapGeneration.BLL.Services;
using MapGeneration.DAL.Entities;
using MapGeneration.DAL.Entities.Users;
using Microsoft.AspNetCore.Mvc;

namespace MapGeneration.API.Controllers;

[ApiController]
[Route("maps")]
public class MapsController : Controller
{
    public const int PreviewMapResolution = 256;
    public const int GenerationPreviewResolution = 512;
    public const int MapViewResolution = 1024;

    private readonly ILogger<MapsController> _logger;
    private readonly IService<MapModel, MapEntity> _mapService;
    private readonly IService<UserModel, UserEntity> _userService;
    private readonly IMapGenerationService _mapGenerationService;

    public MapsController(
        ILogger<MapsController> logger,
        IMapGenerationService mapGenerationService,
        IService<MapModel, MapEntity> mapService,
        IService<UserModel, UserEntity> userService
    )
    {
        _logger = logger;
        _mapService = mapService;
        _userService = userService;
        _mapGenerationService = mapGenerationService;
    }

    [HttpGet("generate/preview/{seed}")]
    public Task<ActionResult> GeneratePreview(string seed)
    {
        Bitmap map = _mapGenerationService.Generate(seed, PreviewMapResolution);
        MemoryStream outputMemoryStream = new MemoryStream();
        map.Save(outputMemoryStream, ImageFormat.Png);
        outputMemoryStream.Seek(0, SeekOrigin.Begin);
        return Task.FromResult<ActionResult>(File(outputMemoryStream, "image/png"));
    }

    [HttpGet("generate/generation/{seed}")]
    public Task<ActionResult> GenerateGeneration(string seed)
    {
        Bitmap map = _mapGenerationService.Generate(seed, GenerationPreviewResolution);
        MemoryStream outputMemoryStream = new MemoryStream();
        map.Save(outputMemoryStream, ImageFormat.Png);
        outputMemoryStream.Seek(0, SeekOrigin.Begin);
        return Task.FromResult<ActionResult>(File(outputMemoryStream, "image/png"));
    }

    [HttpGet("generate/{seed}")]
    public Task<ActionResult> GenerateView(string seed)
    {
        Bitmap map = _mapGenerationService.Generate(seed, MapViewResolution);
        MemoryStream outputMemoryStream = new MemoryStream();
        map.Save(outputMemoryStream, ImageFormat.Png);
        outputMemoryStream.Seek(0, SeekOrigin.Begin);
        return Task.FromResult<ActionResult>(File(outputMemoryStream, "image/png"));
    }

    [HttpGet("")]
    public async Task<IEnumerable<MapModel>> GetAll(
        string sortField,
        int page,
        SortingDirection direction,
        int size)
    {
        return await _mapService.GetPagedAsync(page, size, sortField, direction);
    }

    [HttpGet("author")]
    public async Task<IEnumerable<MapModel>> GetAllByUser(
        Guid userId,
        string sortField,
        int page,
        SortingDirection direction,
        int size)
    {
        return await _mapService.GetPagedAsync(page, size, sortField, direction,
            map =>
                map.Author.Id == userId
        );
    }

    [HttpGet("pages")]
    public Task<ActionResult> GetPagesCount(long pageSize)
    {
        throw new NotImplementedException();
    }

    [HttpPost("create")]
    public async Task<ActionResult> Create(MapModel map)
    {
        bool success = await _mapService.CreateAsync(map);
        if (success)
        {
            return Ok();
        }

        return StatusCode(500);
    }

    [HttpGet("{id}")]
    public Task<ActionResult> Read(Guid id)
    {
        throw new NotImplementedException();
    }

    [HttpPut("update")]
    public Task<ActionResult> Update( /*map dto*/)
    {
        throw new NotImplementedException();
    }
}