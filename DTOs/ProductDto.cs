using System;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class ProductDto
    {
        public Guid SellerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ImageData { get; set; } = string.Empty; 
        public float Discount { get; set; }
    }
}

// namespace api.DTOs
// {
//     public class ProductDto
//     {
//         [Required]
//         public Guid SellerId { get; set; }

//         [Required]
//         [StringLength(100)]
//         public string Name { get; set; } = string.Empty;

//         [StringLength(500)]
//         public string Description { get; set; } = string.Empty;

//         [Required]
//         [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
//         public float Price { get; set; }

//         [Required]
//         [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative integer.")]
//         public int Stock { get; set; }

//         [Required]
//         public string Category { get; set; } = string.Empty;

//         public string ImageData { get; set; } = string.Empty;

//         [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
//         public float Discount { get; set; }
//     }
// }
