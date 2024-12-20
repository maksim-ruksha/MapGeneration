﻿using System.Drawing;
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
    private readonly IMapService _mapService;
    private readonly IService<UserModel, UserEntity> _userService;
    private readonly IMapGenerationService _mapGenerationService;

    public MapsController(
        ILogger<MapsController> logger,
        IMapGenerationService mapGenerationService,
        IService<UserModel, UserEntity> userService,
        IMapService mapService)
    {
        _logger = logger;
        _userService = userService;
        _mapService = mapService;
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
        UserModel userModel = await _userService.FindAsync(userId);
        return await _mapService.GetPagedByUserAsync(page, size, sortField, direction, userModel);
    }

    [HttpGet("pages")]
    public async Task<ActionResult> GetPagesCount(int pageSize)
    {
        return Ok(_mapService.GetPagesCount(pageSize));
    }

    [HttpPost("create")]
    public async Task<ActionResult> Create(MapModel mapModel)
    {
        MapModel createdMap = await _mapService.Create(mapModel);
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
        MapModel updatedMap = await _mapService.UpdateAsync(mapModel);
        return Ok(updatedMap);
    }
}