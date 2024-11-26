using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }
        [HttpGet]


        // public IActionResult GetAll()
        // {
        //     var people = _context.User.FromSql("select * from Users").ToList();
        //                 Console.WriteLine(_context.Database.GetDbConnection().ConnectionString);

        //     return Ok(people);
        // }

        public IList<Users> GetAll()
        {
            return _context.User.FromSqlRaw("SELECT * from Users").ToList();
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var peep = _context.User
                               .FromSqlRaw("SELECT * FROM Users WHERE Id = {0}", id)
                               .FirstOrDefault();

            if (peep == null)
            {
                return NotFound();
            }

            return Ok(peep);
        }


        // [HttpPost]
        // public async Task<IActionResult> Post([FromBody] Users newUser)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     _context.User.Add(newUser);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
        // }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Users newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Generate a new Guid for the Id (if not provided)
            newUser.Id = Guid.NewGuid();

            // Raw SQL query for inserting a new user
            string sqlQuery = "INSERT INTO Users (Id, Name, Email, Password, Address, Role, image) " +
                              "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})";

            // Execute raw SQL command for the insert operation
            await _context.Database.ExecuteSqlRawAsync(sqlQuery,
                newUser.Id,
                newUser.Name,
                newUser.Email,
                newUser.Password,
                newUser.Address,
                newUser.Role,
                newUser.image);

            // Return a "201 Created" response with a reference to the `GetById` method
            return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
        }



        // [HttpDelete("{id}")]
        // public async Task<IActionResult> Delete([FromRoute] Guid id)
        // {
        //     var user = await _context.User.FindAsync(id);
        //     if (user == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.User.Remove(user);
        //     await _context.SaveChangesAsync();

        //     return NoContent(); // HTTP 204 No Content
        // }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Check if the user exists before trying to delete
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Raw SQL query to delete the user by their ID
            string sqlQuery = "DELETE FROM Users WHERE Id = {0}";

            // Execute the raw SQL query
            await _context.Database.ExecuteSqlRawAsync(sqlQuery, id);

            // Return HTTP 204 No Content
            return NoContent();
        }

        // [HttpPut("{id}")]
        // public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] Users updatedUser)
        // {
        //     if (id != updatedUser.Id)
        //     {
        //         return BadRequest("User ID mismatch.");
        //     }

        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     var existingUser = await _context.User.FindAsync(id);
        //     if (existingUser == null)
        //     {
        //         return NotFound();
        //     }
        //     // Update properties of the existing user
        //     existingUser.Name = updatedUser.Name; // Example property, replace with actual properties
        //     existingUser.Email = updatedUser.Email; // Example property, replace with actual properties
        //     existingUser.Password = updatedUser.Password;
        //     existingUser.Address = updatedUser.Address;
        //     existingUser.Role = updatedUser.Role;
        //     // Update other properties as needed

        //     _context.User.Update(existingUser);
        //     await _context.SaveChangesAsync();

        //     return NoContent(); // HTTP 204 No Content
        // }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] Users updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest("User ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Raw SQL query for updating the user
            string sqlQuery = "UPDATE Users SET Name = {1}, Email = {2}, Password = {3}, Address = {4}, Role = {5}, image = {6} WHERE Id = {0}";

            // Execute raw SQL command for the update operation
            var result = await _context.Database.ExecuteSqlRawAsync(sqlQuery,
                id,  // {0} - Id for the WHERE clause
                updatedUser.Name,      // {1} - Name
                updatedUser.Email,     // {2} - Email
                updatedUser.Password,   // {3} - Password
                updatedUser.Address,    // {4} - Address
                updatedUser.Role,       // {5} - Role
                updatedUser.image       // {6} - Image
            );

            if (result == 0)
            {
                return NotFound(); // No rows affected, user not found
            }

            return NoContent(); // HTTP 204 No Content
        }

        [HttpGet("{sellerId}/statistics")]
        public async Task<IActionResult> GetSellerStatistics(Guid sellerId)
        {
            // Validate seller existence
            var seller = await _context.User.FindAsync(sellerId);
            if (seller == null || seller.Role != "Seller")
            {
                return NotFound("Seller not found.");
            }

            // Get the total products sold and earnings for the seller
            var statistics = await _context.OrderItems
                .Where(oi => _context.Product.Any(p => p.Id == oi.ProductId && p.SellerId == sellerId))
                .GroupBy(oi => 1) 
                .Select(g => new
                {
                    TotalProductsSold = g.Sum(oi => oi.OrderedQuantity),
                    TotalEarnings = g.Sum(oi => oi.OrderedQuantity * oi.ItemPrice)
                })
                .FirstOrDefaultAsync();

            // Ensure statistics are not null
            if (statistics == null)
            {
                statistics = new { TotalProductsSold = 0, TotalEarnings = 0.0m };
            }

            return Ok(statistics);
        }



    }
}