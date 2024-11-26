using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class OrderItem
    {
        [Key]
        public Guid ItemId { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ItemPrice { get; set; }

        [Required]
        public int OrderedQuantity { get; set; }

        [Required]
        public string status { get; set; } = string.Empty;

    }
}
