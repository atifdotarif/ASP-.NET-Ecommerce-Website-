using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace api.DTOs
{
    public class ProductWithSellerDTO
{
    [Key]
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }=string.Empty;
    public string Description { get; set; }=string.Empty;
    public float Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; }=string.Empty;
    public string Image { get; set; }=string.Empty;
    public float Discount { get; set; }

    // Seller information
        public Guid SellerId{ get; set; } 

    public string SellerName { get; set; } = string.Empty;
    public string SellerEmail { get; set; } = string.Empty;
}


}
// public class SellerDTO
// {
//     public Guid SellerId { get; set; }
//     public string SellerName { get; set; }=string.Empty;
//     public string SellerEmail { get; set; }=string.Empty;
// }
