﻿using erpsystem.Server.Data;
using erpsystem.Server.Models;
using erpsystem.Server.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
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
        var items = await _context.WarehouseItems
            .Where(i => !i.IsDeleted)
            .ToListAsync();

        var itemDtos = items.Select(item => new WarehouseItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Code = item.Code,
            Quantity = item.Quantity,
            Price = item.Price,
            Category = item.Category,
            Location = item.Location
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
            Location = createDto.Location,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = User?.Identity?.Name ?? "System"
        };

        _context.WarehouseItems.Add(item);

        var log = new OperationLog
        {
            User = User?.Identity?.Name ?? "System",
            Operation = "Dodanie",
            ItemId = item.Id,
            ItemName = item.Name,
            Timestamp = DateTime.UtcNow,
            Details = $"Dodano produkt: {item.Name}, ilość: {item.Quantity}, lokalizacja: {item.Location}"
        };
        _context.OperationLogs.Add(log);

        await _context.SaveChangesAsync();

        var resultDto = new WarehouseItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Code = item.Code,
            Quantity = item.Quantity,
            Price = item.Price,
            Category = item.Category,
            Location = item.Location
        };

        return CreatedAtAction(nameof(GetItems), new { id = item.Id }, resultDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _context.WarehouseItems.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        item.IsDeleted = true;

        var log = new OperationLog
        {
            User = User?.Identity?.Name ?? "System",
            Operation = "Usunięcie",
            ItemId = item.Id,
            ItemName = item.Name,
            Timestamp = DateTime.UtcNow,
            Details = $"Usunięto produkt: {item.Name}"
        };
        _context.OperationLogs.Add(log);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("deleted")]
    public async Task<ActionResult<IEnumerable<WarehouseItemDto>>> GetDeletedItems()
    {
        var deletedItems = await _context.WarehouseItems
            .Where(i => i.IsDeleted)
            .ToListAsync();

        var itemDtos = deletedItems.Select(item => new WarehouseItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Code = item.Code,
            Quantity = item.Quantity,
            Price = item.Price,
            Category = item.Category,
            Location = item.Location
        }).ToList();

        return Ok(itemDtos);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] CreateWarehouseItemDto updateDto)
    {
        var item = await _context.WarehouseItems.FindAsync(id);
        if (item == null || item.IsDeleted)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var changes = GetChangeDetails(item, updateDto);
        var log = new OperationLog
        {
            User = User?.Identity?.Name ?? "System",
            Operation = "Edycja",
            ItemId = item.Id,
            ItemName = item.Name,
            Timestamp = DateTime.UtcNow,
            Details = $"Edytowano produkt: {item.Name}. Zmiany: {changes}"
        };
        _context.OperationLogs.Add(log);

        item.Name = updateDto.Name;
        item.Code = updateDto.Code;
        item.Quantity = updateDto.Quantity;
        item.Price = updateDto.Price;
        item.Category = updateDto.Category;
        item.Location = updateDto.Location;

        await _context.SaveChangesAsync();

        var resultDto = new WarehouseItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Code = item.Code,
            Quantity = item.Quantity,
            Price = item.Price,
            Category = item.Category,
            Location = item.Location
        };

        return Ok(resultDto);
    }

    [HttpPost("move/{id}")]
    public async Task<IActionResult> MoveItem(int id, [FromBody] MoveItemRequest request)
    {
        var item = await _context.WarehouseItems.FindAsync(id);
        if (item == null || item.IsDeleted)
        {
            return NotFound("Produkt nie istnieje lub jest usunięty");
        }

        if (string.IsNullOrEmpty(request.NewLocation))
        {
            return BadRequest("Nowa lokalizacja nie może być pusta");
        }

        var log = new OperationLog
        {
            User = User?.Identity?.Name ?? "System",
            Operation = "Przeniesienie",
            ItemId = item.Id,
            ItemName = item.Name,
            Timestamp = DateTime.UtcNow,
            Details = $"Przeniesiono produkt: {item.Name} z {item.Location} do {request.NewLocation}"
        };
        _context.OperationLogs.Add(log);

        item.Location = request.NewLocation;
        await _context.SaveChangesAsync();

        var resultDto = new WarehouseItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Code = item.Code,
            Quantity = item.Quantity,
            Price = item.Price,
            Category = item.Category,
            Location = item.Location
        };

        return Ok(resultDto);
    }

    [HttpPost("restore/{id}")]
    public async Task<IActionResult> RestoreItem(int id)
    {
        var item = await _context.WarehouseItems.FindAsync(id);
        if (item == null)
        {
            return NotFound("Produkt nie istnieje");
        }

        if (!item.IsDeleted)
        {
            return BadRequest("Produkt nie jest usunięty");
        }

        item.IsDeleted = false;

        var log = new OperationLog
        {
            User = User?.Identity?.Name ?? "System",
            Operation = "Przywrócenie",
            ItemId = item.Id,
            ItemName = item.Name,
            Timestamp = DateTime.UtcNow,
            Details = $"Przywrócono produkt: {item.Name}"
        };
        _context.OperationLogs.Add(log);

        await _context.SaveChangesAsync();

        var resultDto = new WarehouseItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Code = item.Code,
            Quantity = item.Quantity,
            Price = item.Price,
            Category = item.Category,
            Location = item.Location
        };

        return Ok(resultDto);
    }

    [HttpGet("operation-logs")]
    public ActionResult<IEnumerable<OperationLogDto>> GetOperationLogs()
    {
        var logs = _context.OperationLogs
          .Select(l => new OperationLogDto
          {
              Id = l.Id,
              User = l.User,
              Operation = l.Operation,
              ItemId = l.ItemId,
              ItemName = l.ItemName,
              Timestamp = l.Timestamp.ToString("o"),
              Details = l.Details
          })
          .ToList();
        return Ok(logs);
    }

    private string GetChangeDetails(WarehouseItem existingItem, CreateWarehouseItemDto updateDto)
    {
        var changes = new List<string>();
        if (existingItem.Name != updateDto.Name) changes.Add($"Nazwa: {existingItem.Name} -> {updateDto.Name}");
        if (existingItem.Code != updateDto.Code) changes.Add($"Kod: {existingItem.Code} -> {updateDto.Code}");
        if (existingItem.Quantity != updateDto.Quantity) changes.Add($"Ilość: {existingItem.Quantity} -> {updateDto.Quantity}");
        if (existingItem.Price != updateDto.Price) changes.Add($"Cena: {existingItem.Price} -> {updateDto.Price}");
        if (existingItem.Category != updateDto.Category) changes.Add($"Kategoria: {existingItem.Category} -> {updateDto.Category}");
        if (existingItem.Location != updateDto.Location) changes.Add($"Lokalizacja: {existingItem.Location} -> {updateDto.Location}");
        return changes.Count > 0 ? string.Join(", ", changes) : "Brak zmian";
    }
}

public class MoveItemRequest
{
    public string NewLocation { get; set; }
}