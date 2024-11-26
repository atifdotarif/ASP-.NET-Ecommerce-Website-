using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs
{
    public class OrderItemWithSellerDto
{
    public Guid ItemId { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ItemPrice { get; set; }
    public int OrderedQuantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid SellerId { get; set; }
    public string? ProductName { get; set; } 
}

}