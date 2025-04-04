﻿using erpsystem.Server.Data;
using erpsystem.Server.Models;
using erpsystem.Server.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WarehouseController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WarehouseItemDto>>> GetItems()
    {
        var items = await _context.WarehouseItems.ToListAsync();

        var itemDtos = items.Select(item => new WarehouseItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Code = item.Code,
            Quantity = item.Quantity,
            Price = item.Price,
            Category = item.Category
        }).ToList();

        return Ok(itemDtos);
    }

    [HttpPost]
    public async Task<ActionResult<WarehouseItemDto>> AddItem([FromBody] CreateWarehouseItemDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var item = new WarehouseItem
        {
            Name = createDto.Name,
            Code = createDto.Code,
            Quantity = createDto.Quantity,
            Price = createDto.Price,
            Category = createDto.Category,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = User?.Identity?.Name ?? "System"
        };

        _context.WarehouseItems.Add(item);
        await _context.SaveChangesAsync();

        var resultDto = new WarehouseItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Code = item.Code,
            Quantity = item.Quantity,
            Price = item.Price,
            Category = item.Category
        };

        return CreatedAtAction(nameof(GetItems), new { id = item.Id }, resultDto);
    }
}