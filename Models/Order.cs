using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public Users? User { get; set; } // Navigation property

        [ForeignKey("Address")]
        public Guid AddressId { get; set; }

        public DeliveryAddress? Address { get; set; } // Navigation property
        
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public DateTime Date { get; set; }
    }
}
