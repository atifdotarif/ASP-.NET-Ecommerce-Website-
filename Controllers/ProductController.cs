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
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public ProductController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductWithSellerDTO>>> GetAll()
        {
            // Raw SQL query to retrieve products with their seller details
            string sqlQuery = @"
            SELECT 
                p.Id AS ProductId,
                p.Name AS ProductName,
                p.Description,
                p.Price,
                p.Stock,
                p.Category,
                p.Image,
                p.Discount,
                u.Id AS SellerId,
                u.Name AS SellerName,
                u.Email AS SellerEmail
            FROM Product p
            LEFT JOIN Users u ON p.SellerId = u.Id";

            // Execute the raw SQL command and map to ProductSellerResult
            var results = await _context.Set<ProductWithSellerDTO>()
                .FromSqlRaw(sqlQuery)
                .AsNoTracking()
                .ToListAsync();

            // Map to ProductWithSellerDTO
            var products = results.Select(p => new ProductWithSellerDTO
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Category = p.Category,
                Image = p.Image,
                Discount = p.Discount,
                SellerId = p.SellerId,
                SellerName = p.SellerName,
                SellerEmail = p.SellerEmail
            }).ToList();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductWithSellerDTO>> GetById([FromRoute] Guid id)
        {
            // Raw SQL query to retrieve a specific product with seller details
            string sqlQuery = @"
            SELECT 
                p.Id AS ProductId,
                p.Name AS ProductName,
                p.Description,
                p.Price,
                p.Stock,
                p.Category,
                p.Image,
                p.Discount,
                u.Id AS SellerId,
                u.Name AS SellerName,
                u.Email AS SellerEmail
            FROM Product p
            LEFT JOIN Users u ON p.SellerId = u.Id
            WHERE p.Id = {0}";

            // Execute the raw SQL command and get the result
            var product = await _context.Set<ProductWithSellerDTO>()
                .FromSqlRaw(sqlQuery, id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            // Check if product exists
            if (product == null)
            {
                return NotFound();
            }

            // Map to ProductWithSellerDTO
            var productWithSeller = new ProductWithSellerDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Category = product.Category,
                Image = product.Image,
                Discount = product.Discount,
                SellerId = product.SellerId,
                SellerName = product.SellerName,
                SellerEmail = product.SellerEmail
            };

            return Ok(productWithSeller);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate if the seller exists and has the correct role
            var seller = await _context.User
                .FirstOrDefaultAsync(u => u.Id == productDto.SellerId && u.Role == "Seller");

            if (seller == null)
            {
                return NotFound("Seller with the specified ID and role 'Seller' not found.");
            }

            // Generate a new GUID for the product Id
            var productId = Guid.NewGuid();

            // Prepare the raw SQL query for inserting the product
            string sqlQuery = @"
    INSERT INTO Product (Id, SellerId, Name, Description, Price, Stock, Category, Image, Discount)
    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";



            // Execute the raw SQL command
            await _context.Database.ExecuteSqlRawAsync(sqlQuery,
                productId, // New GUID for Id
                productDto.SellerId,
                productDto.Name,
                productDto.Description,
                productDto.Price,
                productDto.Stock,
                productDto.Category,
                productDto.ImageData, // Store image data as a string
                productDto.Discount);

            // Prepare the DTO to return
            var createdProductDto = new ProductDto
            {
                SellerId = productDto.SellerId,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                Category = productDto.Category,
                ImageData = productDto.ImageData, // Return the image data
                Discount = productDto.Discount
            };

            // Returning the created product with its Id
            return CreatedAtAction(nameof(GetAll), new { id = productId }, createdProductDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Prepare the raw SQL query for deleting the product
            string sqlQuery = "DELETE FROM Product WHERE Id = {0}";

            // Execute the raw SQL command
            var result = await _context.Database.ExecuteSqlRawAsync(sqlQuery, id);

            // Check if any rows were affected (i.e., the product was found and deleted)
            if (result == 0)
            {
                return NotFound(); // No rows affected means the product was not found
            }

            return NoContent(); // HTTP 204 No Content
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 Bad Request if model state is invalid
            }

            // Check if the product exists
            var existingProduct = await _context.Product.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found."); // Return 404 if product does not exist
            }

            // Validate if the seller exists and has the correct role
            var seller = await _context.User
                .FirstOrDefaultAsync(u => u.Id == productDto.SellerId && u.Role == "Seller");
            if (seller == null)
            {
                return NotFound("Seller with the specified ID and role 'Seller' not found."); // Return 404 if seller is invalid
            }

            // Raw SQL query to update the product
            string sqlQuery = @"
        UPDATE Product
        SET 
            SellerId = {1},
            Name = {2},
            Description = {3},
            Price = {4},
            Stock = {5},
            Category = {6},
            Image = {7},
            Discount = {8}
        WHERE Id = {0}";

            // Execute the raw SQL command
            var result = await _context.Database.ExecuteSqlRawAsync(sqlQuery,
                id, // Product ID to update
                productDto.SellerId,
                productDto.Name,
                productDto.Description,
                productDto.Price,
                productDto.Stock,
                productDto.Category,
                productDto.ImageData,
                productDto.Discount);

            // Check if the query affected any rows
            if (result == 0)
            {
                return NotFound($"Product with ID {id} not found after query execution."); // Return 404 if no rows were updated
            }

            // Return the updated product details
            return Ok(new ProductDto
            {
                SellerId = productDto.SellerId,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                Category = productDto.Category,
                ImageData = productDto.ImageData,
                Discount = productDto.Discount
            });
        }

        [HttpPut("subtract-stock")]
        public async Task<IActionResult> SubtractStock([FromBody] StockUpdateDto stockUpdate)
        {
            if (stockUpdate.QuantityToSubtract <= 0)
            {
                return BadRequest(new { message = "Quantity to subtract must be greater than 0." });
            }

            var product = await _context.Product.FindAsync(stockUpdate.ProductId);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {stockUpdate.ProductId} not found." });
            }

            if (product.Stock < stockUpdate.QuantityToSubtract)
            {
                return BadRequest(new { message = "Insufficient stock." });
            }

            product.Stock -= stockUpdate.QuantityToSubtract;
            await _context.SaveChangesAsync();

            // Return a JSON response
            return Ok(new
            {
                message = "Stock updated successfully.",
                productId = stockUpdate.ProductId,
                remainingStock = product.Stock
            });
        }




    }
}