using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using api.Models;

public class Products
{
    [Key]
    public Guid Id { get; set; }
    [Column("SellerId")]
    public Guid SellerId { get; set; }
    
    [ForeignKey("SellerId")]
    public Users? Seller { get; set; } // Navigation property

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public float Discount { get; set; }
}
