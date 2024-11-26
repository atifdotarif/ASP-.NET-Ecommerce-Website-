using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs
{
    public class OrderItemWithBuyerDto
{
    public Guid ItemId { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public decimal ItemPrice { get; set; }
    public int OrderedQuantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid BuyerId { get; set; }
    public string? ProductName { get; set; }
}


}