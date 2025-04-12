using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using erpsystem.Server.Models;
using erpsystem.Server.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using erpsystem.Server.Data;

namespace erpsystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(bool showDeleted = false)
        {
            var orders = await _context.Orders
                .Include(o => o.Contractor)
                .Include(o => o.Items).ThenInclude(i => i.WarehouseItem)
                .Where(o => o.IsDeleted == showDeleted)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    ContractorId = o.ContractorId,
                    Contractor = new ContractorDTO
                    {
                        Id = o.Contractor.Id,
                        Name = o.Contractor.Name,
                        Email = o.Contractor.Email,
                        TaxId = o.Contractor.TaxId
                    },
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    TotalNetAmount = o.TotalNetAmount,
                    TotalGrossAmount = o.TotalGrossAmount,
                    Discount = o.Discount,
                    Currency = o.Currency,
                    Status = o.Status,
                    Notes = o.Notes,
                    IsDeleted = o.IsDeleted,
                    Items = o.Items.Select(i => new OrderItemDto
                    {
                        Id = i.Id,
                        WarehouseItemId = i.WarehouseItemId,
                        ItemName = i.ItemName,
                        Quantity = i.Quantity,
                        UnitPriceNet = i.UnitPriceNet,
                        UnitPriceGross = i.UnitPriceGross,
                        VatRate = i.VatRate,
                        Discount = i.Discount
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Contractor)
                .Include(o => o.Items).ThenInclude(i => i.WarehouseItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var orderDto = new OrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                ContractorId = order.ContractorId,
                Contractor = new ContractorDTO
                {
                    Id = order.Contractor.Id,
                    Name = order.Contractor.Name,
                    Email = order.Contractor.Email,
                    TaxId = order.Contractor.TaxId
                },
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate,
                TotalNetAmount = order.TotalNetAmount,
                TotalGrossAmount = order.TotalGrossAmount,
                Discount = order.Discount,
                Currency = order.Currency,
                Status = order.Status,
                Notes = order.Notes,
                IsDeleted = order.IsDeleted,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    WarehouseItemId = i.WarehouseItemId,
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    UnitPriceNet = i.UnitPriceNet,
                    UnitPriceGross = i.UnitPriceGross,
                    VatRate = i.VatRate,
                    Discount = i.Discount
                }).ToList()
            };

            return Ok(orderDto);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            if (!_context.Contractors.Any(c => c.Id == createOrderDto.ContractorId && !c.IsDeleted))
                return BadRequest("Nieprawidłowy kontrahent.");

            foreach (var orderItem in createOrderDto.Items)
            {
                var warehouseItem = await _context.WarehouseItems
                    .FirstOrDefaultAsync(w => w.Id == orderItem.WarehouseItemId && !w.IsDeleted);
                if (warehouseItem == null)
                    return BadRequest($"Produkt {orderItem.ItemName} nie istnieje.");
                if (warehouseItem.Quantity < orderItem.Quantity)
                    return BadRequest($"Niewystarczająca ilość produktu {orderItem.ItemName} w magazynie.");
            }

            var order = new Order
            {
                OrderNumber = createOrderDto.OrderNumber,
                ContractorId = createOrderDto.ContractorId,
                OrderDate = createOrderDto.OrderDate,
                DeliveryDate = createOrderDto.DeliveryDate,
                TotalNetAmount = createOrderDto.TotalNetAmount,
                TotalGrossAmount = createOrderDto.TotalGrossAmount,
                Discount = createOrderDto.Discount,
                Currency = createOrderDto.Currency,
                Status = createOrderDto.Status,
                Notes = createOrderDto.Notes,
                IsDeleted = false,
                Items = createOrderDto.Items.Select(i => new OrderItem
                {
                    WarehouseItemId = i.WarehouseItemId,
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    UnitPriceNet = i.UnitPriceNet,
                    UnitPriceGross = i.UnitPriceGross,
                    VatRate = i.VatRate,
                    Discount = i.Discount
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var orderItem in order.Items)
            {
                var warehouseItem = await _context.WarehouseItems.FindAsync(orderItem.WarehouseItemId);
                warehouseItem.Quantity -= orderItem.Quantity; 
            }
            await _context.SaveChangesAsync();

            var orderDto = await _context.Orders
                .Include(o => o.Contractor)
                .Include(o => o.Items).ThenInclude(i => i.WarehouseItem)
                .Where(o => o.Id == order.Id)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    ContractorId = o.ContractorId,
                    Contractor = new ContractorDTO
                    {
                        Id = o.Contractor.Id,
                        Name = o.Contractor.Name,
                        Email = o.Contractor.Email,
                        TaxId = o.Contractor.TaxId
                    },
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    TotalNetAmount = o.TotalNetAmount,
                    TotalGrossAmount = o.TotalGrossAmount,
                    Discount = o.Discount,
                    Currency = o.Currency,
                    Status = o.Status,
                    Notes = o.Notes,
                    IsDeleted = o.IsDeleted,
                    Items = o.Items.Select(i => new OrderItemDto
                    {
                        Id = i.Id,
                        WarehouseItemId = i.WarehouseItemId,
                        ItemName = i.ItemName,
                        Quantity = i.Quantity,
                        UnitPriceNet = i.UnitPriceNet,
                        UnitPriceGross = i.UnitPriceGross,
                        VatRate = i.VatRate,
                        Discount = i.Discount
                    }).ToList()
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDto updateOrderDto)
        {
            if (id != updateOrderDto.Id)
                return BadRequest("Niezgodny identyfikator.");

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null || order.IsDeleted)
                return NotFound();

            if (!_context.Contractors.Any(c => c.Id == updateOrderDto.ContractorId && !c.IsDeleted))
                return BadRequest("Nieprawidłowy kontrahent.");

            foreach (var orderItem in order.Items)
            {
                var warehouseItem = await _context.WarehouseItems.FindAsync(orderItem.WarehouseItemId);
                warehouseItem.Quantity += orderItem.Quantity;
            }

            foreach (var orderItem in updateOrderDto.Items)
            {
                var warehouseItem = await _context.WarehouseItems
                    .FirstOrDefaultAsync(w => w.Id == orderItem.WarehouseItemId && !w.IsDeleted);
                if (warehouseItem == null)
                    return BadRequest($"Produkt {orderItem.ItemName} nie istnieje.");
                if (warehouseItem.Quantity < orderItem.Quantity)
                    return BadRequest($"Niewystarczająca ilość produktu {orderItem.ItemName} w magazynie.");
            }

            order.OrderNumber = updateOrderDto.OrderNumber;
            order.ContractorId = updateOrderDto.ContractorId;
            order.OrderDate = updateOrderDto.OrderDate;
            order.DeliveryDate = updateOrderDto.DeliveryDate;
            order.TotalNetAmount = updateOrderDto.TotalNetAmount;
            order.TotalGrossAmount = updateOrderDto.TotalGrossAmount;
            order.Discount = updateOrderDto.Discount;
            order.Currency = updateOrderDto.Currency;
            order.Status = updateOrderDto.Status;
            order.Notes = updateOrderDto.Notes;

            _context.OrderItems.RemoveRange(order.Items);
            order.Items = updateOrderDto.Items.Select(i => new OrderItem
            {
                Id = i.Id,
                OrderId = order.Id,
                WarehouseItemId = i.WarehouseItemId,
                ItemName = i.ItemName,
                Quantity = i.Quantity,
                UnitPriceNet = i.UnitPriceNet,
                UnitPriceGross = i.UnitPriceGross,
                VatRate = i.VatRate,
                Discount = i.Discount
            }).ToList();

            foreach (var orderItem in order.Items)
            {
                var warehouseItem = await _context.WarehouseItems.FindAsync(orderItem.WarehouseItemId);
                warehouseItem.Quantity -= orderItem.Quantity; 
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null || order.IsDeleted)
                return NotFound();

            order.IsDeleted = true;

            foreach (var orderItem in order.Items)
            {
                var warehouseItem = await _context.WarehouseItems.FindAsync(orderItem.WarehouseItemId);
                warehouseItem.Quantity += orderItem.Quantity; 
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null || !order.IsDeleted)
                return NotFound();

            foreach (var orderItem in order.Items)
            {
                var warehouseItem = await _context.WarehouseItems
                    .FirstOrDefaultAsync(w => w.Id == orderItem.WarehouseItemId && !w.IsDeleted);
                if (warehouseItem == null || warehouseItem.Quantity < orderItem.Quantity)
                    return BadRequest($"Niewystarczająca ilość produktu {orderItem.ItemName} w magazynie.");
            }

            order.IsDeleted = false;

            foreach (var orderItem in order.Items)
            {
                var warehouseItem = await _context.WarehouseItems.FindAsync(orderItem.WarehouseItemId);
                warehouseItem.Quantity -= orderItem.Quantity; 
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}