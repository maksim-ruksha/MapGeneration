﻿using Microsoft.AspNetCore.Mvc;

namespace MapGeneration.API.Controllers;

[ApiController]
[Route("maps")]
public class MapsController: Controller
{
    [HttpGet("generate/{seed}")]
    public Task<ActionResult> Generate(string seed)
    {
        throw new NotImplementedException();
    }

    [HttpGet("generate/hd/{seed}")]
    public Task<ActionResult> GenerateHd(string seed)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("")]
    public Task<ActionResult> GetAll(string sortField,
        int page,
        string direction,
        int size)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("author")]
    public Task<ActionResult> GetAllByUser(long userId, string sortField, int page, string direction, int size)
    {
        throw new NotImplementedException();
    }

    [HttpGet("pages")]
    public Task<ActionResult> GetPagesCount(long pageSize)
    {
        throw new NotImplementedException();
    }

    [HttpPost("create")]
    public Task<ActionResult> Create( /*map dto*/)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public Task<ActionResult> Read(long id)
    {
        throw new NotImplementedException();
    }

    [HttpPut("update")]
    public Task<ActionResult> Update( /*map dto*/)
    {
        throw new NotImplementedException();
    }
}