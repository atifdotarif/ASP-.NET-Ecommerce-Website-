using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/orderitems")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public OrderItemController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("all-order-items-with-seller")]
        public async Task<ActionResult<IEnumerable<OrderItemWithSellerDto>>> GetAllOrderItems(
    [FromQuery] Guid? sellerId,
    [FromQuery] string status)
        {
            var query = @"
    SELECT 
        oi.ItemId,
        oi.OrderId,
        oi.ProductId,
        oi.ItemPrice,
        oi.OrderedQuantity,
        oi.status AS Status,
        p.SellerId,
        p.Name AS ProductName
    FROM OrderItems oi
    INNER JOIN Product p ON oi.ProductId = p.Id
    WHERE (@SellerId IS NULL OR p.SellerId = @SellerId)
      AND (@Status IS NULL OR oi.status = @Status)";

            var parameters = new[]
            {
        new SqlParameter("@SellerId", sellerId ?? (object)DBNull.Value),
        new SqlParameter("@Status", string.IsNullOrEmpty(status) ? (object)DBNull.Value : status)
    };

            var result = await _context.OrderItemWithSellerDtos
                .FromSqlRaw(query, parameters)
                .ToListAsync();

            return Ok(result);
        }



        [HttpGet("order-items-with-buyers")]
        public async Task<ActionResult<IEnumerable<OrderItemWithBuyerDto>>> GetOrderItemsWithBuyer(
    [FromQuery] Guid? buyerId,
    [FromQuery] string status)
        {
            var query = @"
        SELECT 
            oi.ItemId,
            oi.OrderId,
            oi.ProductId,
            oi.ItemPrice,
            oi.OrderedQuantity,
            oi.status AS Status,
            o.UserId AS BuyerId,
            p.Name AS ProductName
        FROM OrderItems oi
        INNER JOIN Orders o ON oi.OrderId = o.OrderId
        LEFT JOIN Product p ON oi.ProductId = p.Id
        WHERE (@BuyerId IS NULL OR o.UserId = @BuyerId)
          AND (@Status IS NULL OR oi.status = @Status)";

            var parameters = new[]
            {
        new SqlParameter("@BuyerId", buyerId ?? (object)DBNull.Value),
        new SqlParameter("@Status", string.IsNullOrEmpty(status) ? (object)DBNull.Value : status)
    };

            var result = await _context.OrderItemWithBuyerDtos
                .FromSqlRaw(query, parameters)
                .ToListAsync();

            return Ok(result);
        }








        // GET: api/orderitems/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetOrderItemById(Guid id)
        {
            string sqlQuery = "SELECT * FROM OrderItems WHERE ItemId = {0}";
            var orderItem = await _context.Set<OrderItem>()
                .FromSqlRaw(sqlQuery, id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (orderItem == null)
            {
                return NotFound();
            }

            return Ok(orderItem);
        }

        // POST: api/orderitems
        [HttpPost]
        public async Task<IActionResult> CreateOrderItem([FromBody] OrderItem orderItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (orderItem.OrderedQuantity <= 0)
            {
                return BadRequest("Ordered quantity must be greater than zero.");
            }

            orderItem.ItemId = Guid.NewGuid();

            string sqlQuery = @"
    INSERT INTO OrderItems (ItemId, OrderId, ProductId, ItemPrice, OrderedQuantity, status)
    VALUES (@ItemId, @OrderId, @ProductId, @ItemPrice, @OrderedQuantity, @Status)";

            var parameters = new[]
            {
        new SqlParameter("@ItemId", orderItem.ItemId),
        new SqlParameter("@OrderId", orderItem.OrderId),
        new SqlParameter("@ProductId", orderItem.ProductId),
        new SqlParameter("@ItemPrice", orderItem.ItemPrice),
        new SqlParameter("@OrderedQuantity", orderItem.OrderedQuantity),
        new SqlParameter("@Status", orderItem.status)
    };

            await _context.Database.ExecuteSqlRawAsync(sqlQuery, parameters);

            return CreatedAtAction(nameof(GetOrderItemById), new { id = orderItem.ItemId }, orderItem);
        }



        // DELETE: api/orderitems/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(Guid id)
        {
            string sqlQuery = "DELETE FROM OrderItems WHERE ItemId = {0}";

            var result = await _context.Database.ExecuteSqlRawAsync(sqlQuery, id);

            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // PUT: api/orderitems/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(Guid id, [FromBody] OrderItem updatedOrderItem)
        {
            if (id != updatedOrderItem.ItemId)
            {
                return BadRequest("Item ID mismatch.");
            }

            string sqlQuery = @"
        UPDATE OrderItems 
        SET 
            OrderId = @OrderId, 
            ProductId = @ProductId, 
            ItemPrice = @ItemPrice, 
            OrderedQuantity = @OrderedQuantity,
            status = @Status 
        WHERE ItemId = @ItemId";

            var parameters = new[]
            {
        new SqlParameter("@ItemId", id),
        new SqlParameter("@OrderId", updatedOrderItem.OrderId),
        new SqlParameter("@ProductId", updatedOrderItem.ProductId),
        new SqlParameter("@ItemPrice", updatedOrderItem.ItemPrice),
        new SqlParameter("@OrderedQuantity", updatedOrderItem.OrderedQuantity),
        new SqlParameter("@Status", updatedOrderItem.status),
    };

            var result = await _context.Database.ExecuteSqlRawAsync(sqlQuery, parameters);

            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
