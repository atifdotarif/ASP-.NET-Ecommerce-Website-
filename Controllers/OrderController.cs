using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public OrderController(ApplicationDBContext context)
        {
            _context = context;
        }

        // Get all orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .FromSqlRaw("SELECT * FROM Orders")
                .AsNoTracking()
                .ToListAsync();

            return Ok(orders);
        }

        // Get order by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(Guid id)
        {
            var order = await _context.Orders
                .FromSqlRaw("SELECT * FROM Orders WHERE OrderId = {0}", id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // Create new order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userExists = await _context.User
                .FromSqlRaw("SELECT * FROM Users WHERE Id = {0}", orderDto.UserId)
                .AnyAsync();

            var addressExists = await _context.Addresses
                .FromSqlRaw("SELECT * FROM Addresses WHERE AddressId = {0}", orderDto.AddressId)
                .AnyAsync();

            if (!userExists || !addressExists)
            {
                return NotFound("Invalid User or Address ID.");
            }

            var orderId = Guid.NewGuid();

            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO Orders (OrderId, UserId, AddressId, TotalPrice, Date) VALUES ({0}, {1}, {2}, {3}, {4})",
                orderId, orderDto.UserId, orderDto.AddressId, orderDto.TotalPrice, DateTime.UtcNow);

            var response = new
            {
                orderId,
                orderDto.UserId,
                orderDto.AddressId,
                orderDto.TotalPrice,
                Date = DateTime.UtcNow
            };

            return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, response);
        }

        // Update order
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] OrderDto orderDto)
        {
            if (id != orderDto.OrderId)
            {
                return BadRequest("Order ID mismatch.");
            }

            var orderExists = await _context.Orders
                .FromSqlRaw("SELECT * FROM Orders WHERE OrderId = {0}", id)
                .AnyAsync();

            if (!orderExists)
            {
                return NotFound();
            }

            var userExists = await _context.User
                .FromSqlRaw("SELECT * FROM Users WHERE Id = {0}", orderDto.UserId)
                .AnyAsync();

            var addressExists = await _context.Addresses
                .FromSqlRaw("SELECT * FROM Addresses WHERE AddressId = {0}", orderDto.AddressId)
                .AnyAsync();

            if (!userExists || !addressExists)
            {
                return NotFound("Invalid User or Address ID.");
            }

            await _context.Database.ExecuteSqlRawAsync(
                "UPDATE Orders SET UserId = {0}, AddressId = {1}, TotalPrice = {2} WHERE OrderId = {3}",
                orderDto.UserId, orderDto.AddressId, orderDto.TotalPrice, id);

            return NoContent();
        }

        // Delete order
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var orderExists = await _context.Orders
                .FromSqlRaw("SELECT * FROM Orders WHERE OrderId = {0}", id)
                .AnyAsync();

            if (!orderExists)
            {
                return NotFound();
            }

            await _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM Orders WHERE OrderId = {0}", id);

            return NoContent();
        }
    }
}
