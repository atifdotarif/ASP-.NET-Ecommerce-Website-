using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/addresses")]
    [ApiController]
    public class DeliveryAddressController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public DeliveryAddressController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryAddress>>> GetAllAddresses()
        {
            string sqlQuery = "SELECT * FROM Addresses";
            var addresses = await _context.Set<DeliveryAddress>()
                .FromSqlRaw(sqlQuery)
                .AsNoTracking()
                .ToListAsync();

            return Ok(addresses);
        }

        // GET: api/addresses/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryAddress>> GetAddressById(Guid id)
        {
            string sqlQuery = "SELECT * FROM Addresses WHERE AddressId = {0}";
            var address = await _context.Set<DeliveryAddress>()
                .FromSqlRaw(sqlQuery, id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }

        // POST: api/addresses
        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] DeliveryAddress address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            address.AddressId = Guid.NewGuid();

            string sqlQuery = @"
                INSERT INTO Addresses (AddressId, Street, City, State, ZipCode, PhoneNumber, UserId)
                VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})";

            await _context.Database.ExecuteSqlRawAsync(sqlQuery,
                address.AddressId,
                address.Street,
                address.City,
                address.State,
                address.ZipCode,
                address.PhoneNumber,
                address.UserId);

            return CreatedAtAction(nameof(GetAddressById), new { id = address.AddressId }, address);
        }

        // DELETE: api/addresses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            string sqlQuery = "DELETE FROM Addresses WHERE AddressId = {0}";

            var result = await _context.Database.ExecuteSqlRawAsync(sqlQuery, id);

            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // PUT: api/addresses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] DeliveryAddress updatedAddress)
        {
            if (id != updatedAddress.AddressId)
            {
                return BadRequest("Address ID mismatch.");
            }

            string sqlQuery = @"
                UPDATE Addresses
                SET 
                    Street = {1}, 
                    City = {2}, 
                    State = {3}, 
                    ZipCode = {4}, 
                    PhoneNumer = {5}, 
                    UserId = {6}
                WHERE AddressId = {0}";

            var result = await _context.Database.ExecuteSqlRawAsync(sqlQuery,
                id,
                updatedAddress.Street,
                updatedAddress.City,
                updatedAddress.State,
                updatedAddress.ZipCode,
                updatedAddress.PhoneNumber,
                updatedAddress.UserId);

            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
