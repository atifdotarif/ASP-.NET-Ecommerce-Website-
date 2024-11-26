using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs
{
    public class OrderDto
{
    public Guid OrderId { get; set; } // Required for update
    public Guid UserId { get; set; }
    public Guid AddressId { get; set; }
    public decimal TotalPrice { get; set; }
}

}