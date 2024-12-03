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

    [HttpGet("generate/hd/{seed}")]
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
    public async Task<ActionResult> GetPagesCount(long pageSize)
    {
        long totalMaps = await _mapService.Count();
        decimal d = (decimal) totalMaps / pageSize;
        return Ok(Math.Ceiling(d));
    }

    [HttpPost("create")]
    public async Task<ActionResult> Create(MapModel mapModel)
    {
        UserModel user = await _userService.FindAsync(mapModel.Author.Id);
        mapModel.Author = user;
        mapModel.DateTime = DateTime.UtcNow;

        bool success = await _mapService.CreateAsync(mapModel);
        if (!success)
        {
            return StatusCode(500);
        }
        
        MapModel createdMap = await _mapService.GetFirstAsync(map => map.Author.Id == user.Id);
        return Ok(createdMap);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        MapModel map = await _mapService.FindAsync(id);
        return Ok(map);
    }

    [HttpPut("update")]
    public async Task<ActionResult> Update(MapModel mapModel)
    {
        bool success = await _mapService.UpdateAsync(mapModel);
        if (!success)
        {
            return StatusCode(500);
        }

        MapModel updatedMap = await _mapService.FindAsync(mapModel.Id);
        return Ok(updatedMap);
        //throw new NotImplementedException();
    }
}