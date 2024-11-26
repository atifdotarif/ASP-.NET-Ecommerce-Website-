using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/orderhistory")]
    [ApiController]
    public class OrderHistoryController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public OrderHistoryController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/orderhistory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderHistory>>> GetAllOrderHistories()
        {
            string sqlQuery = "SELECT * FROM orderHistories";
            var orderHistories = await _context.Set<OrderHistory>()
                .FromSqlRaw(sqlQuery)
                .AsNoTracking()
                .ToListAsync();

            return Ok(orderHistories);
        }

        [HttpGet("userId")]
        public async Task<ActionResult<IEnumerable<OrderHistory>>> GetOrderHistoriesByUserId(Guid userId)
        {
            string sqlQuery = "SELECT * FROM orderHistories WHERE UserId = {0}";
            var orderHistories = await _context.Set<OrderHistory>()
                .FromSqlRaw(sqlQuery, userId)
                .AsNoTracking()
                .ToListAsync();

            if (orderHistories == null || !orderHistories.Any())
            {
                return NotFound($"No order histories found for UserId: {userId}");
            }

            return Ok(orderHistories);
        }


        [HttpGet("orders-delivered-history")]
        public async Task<ActionResult<IEnumerable<OrderHistory>>> GetAllOrderHistoriesDelivered()
        {
            string sqlQuery = "SELECT * FROM orderHistories where Status='Delivered'";
            var orderHistories = await _context.Set<OrderHistory>()
                .FromSqlRaw(sqlQuery)
                .AsNoTracking()
                .ToListAsync();

            return Ok(orderHistories);
        }

        // GET: api/orderhistory/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderHistory>> GetOrderHistoryById(Guid id)
        {
            string sqlQuery = "SELECT * FROM orderHistories WHERE HistoryId = {0}";
            var orderHistory = await _context.Set<OrderHistory>()
                .FromSqlRaw(sqlQuery, id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (orderHistory == null)
            {
                return NotFound();
            }

            return Ok(orderHistory);
        }

        // POST: api/orderhistory
        [HttpPost]
        public async Task<IActionResult> CreateOrderHistory([FromBody] OrderHistory orderHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            orderHistory.HistoryId = Guid.NewGuid();

            string sqlQuery = @"
                INSERT INTO orderHistories (HistoryId, Status, UserId, OrderId)
                VALUES ({0}, {1}, {2}, {3})";

            await _context.Database.ExecuteSqlRawAsync(sqlQuery,
                orderHistory.HistoryId,
                orderHistory.Status,
                orderHistory.UserId,
                orderHistory.OrderId);

            return CreatedAtAction(nameof(GetOrderHistoryById), new { id = orderHistory.HistoryId }, orderHistory);
        }

        // DELETE: api/orderhistory/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderHistory(Guid id)
        {
            string sqlQuery = "DELETE FROM orderHistories WHERE HistoryId = {0}";

            var result = await _context.Database.ExecuteSqlRawAsync(sqlQuery, id);

            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("UpdateOrderHistoriesStatus")]
        public async Task<IActionResult> UpdateOrderHistoriesStatus()
        {
            try
            {
                // Check all OrderHistories in the database
                var checkAndUpdateQuery = @"
            UPDATE OrderHistories
            SET Status = 'Delivered'
            WHERE OrderId IN (
                SELECT DISTINCT oh.OrderId
                FROM OrderHistories oh
                WHERE NOT EXISTS (
                    SELECT 1 
                    FROM OrderItems oi
                    WHERE oi.OrderId = oh.OrderId AND oi.status != 'Delivered'
                )
            )
            AND Status != 'Delivered'";

                // Execute the query
                int rowsAffected = await _context.Database.ExecuteSqlRawAsync(checkAndUpdateQuery);

                if (rowsAffected > 0)
                {
                    return Ok(new { Message = $"{rowsAffected} OrderHistory entries updated to 'Delivered'." });
                }

                return Ok(new { Message = "No OrderHistory entries needed updates." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
            }
        }


        // PUT: api/orderhistory/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderHistory(Guid id, [FromBody] OrderHistory updatedOrderHistory)
        {
            if (id != updatedOrderHistory.HistoryId)
            {
                return BadRequest("History ID mismatch.");
            }

            string sqlQuery = @"
                UPDATE orderHistories
                SET 
                    Status = {1},
                    UserId = {2},
                    OrderId = {3}
                WHERE HistoryId = {0}";

            var result = await _context.Database.ExecuteSqlRawAsync(sqlQuery,
                id,
                updatedOrderHistory.Status,
                updatedOrderHistory.UserId,
                updatedOrderHistory.OrderId);

            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
