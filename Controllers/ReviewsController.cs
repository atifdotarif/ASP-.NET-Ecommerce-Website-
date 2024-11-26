using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ReviewsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/reviews
        // GET: api/reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllReviews([FromQuery] Guid? productId)
        {
            if (productId.HasValue)
            {
                string sqlQuery = "SELECT * FROM Reviews WHERE ProductId = {0}";
                var reviews = await _context.Set<Review>()
                    .FromSqlRaw(sqlQuery, productId.Value)
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(reviews);
            }
            else
            {
                string sqlQuery = "SELECT * FROM Reviews";
                var reviews = await _context.Set<Review>()
                    .FromSqlRaw(sqlQuery)
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(reviews);
            }
        }


        // GET: api/reviews/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReviewById(Guid id)
        {
            string sqlQuery = "SELECT * FROM Reviews WHERE ReviewId = {0}";
            var review = await _context.Set<Review>()
                .FromSqlRaw(sqlQuery, id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (review == null)
            {
                return NotFound();
            }

            return Ok(review);
        }

        // POST: api/reviews
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            review.ReviewId = Guid.NewGuid();

            string sqlQuery = @"
                INSERT INTO Reviews (ReviewId, UserId, ProductId, Comments, Stars)
                VALUES ({0}, {1}, {2}, {3}, {4})";

            await _context.Database.ExecuteSqlRawAsync(sqlQuery,
                review.ReviewId,
                review.UserId,
                review.ProductId,
                review.Comments,
                review.Stars);

            return CreatedAtAction(nameof(GetReviewById), new { id = review.ReviewId }, review);
        }

        // DELETE: api/reviews/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            string sqlQuery = "DELETE FROM Reviews WHERE ReviewId = {0}";

            var result = await _context.Database.ExecuteSqlRawAsync(sqlQuery, id);

            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // PUT: api/reviews/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(Guid id, [FromBody] Review updatedReview)
        {
            if (id != updatedReview.ReviewId)
            {
                return BadRequest("Review ID mismatch.");
            }

            string sqlQuery = @"
                UPDATE Reviews
                SET 
                    UserId = {1},
                    ProductId = {2},
                    Comments = {3},
                    Stars = {4}
                WHERE ReviewId = {0}";

            var result = await _context.Database.ExecuteSqlRawAsync(sqlQuery,
                id,
                updatedReview.UserId,
                updatedReview.ProductId,
                updatedReview.Comments,
                updatedReview.Stars);

            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
