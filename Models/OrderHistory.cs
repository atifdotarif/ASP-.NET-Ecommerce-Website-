using System;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class OrderHistory
    {
        [Key]
        public Guid HistoryId { get; set; }

        [Required]
        public string Status { get; set; }=string.Empty;

        [Required]
        public Guid UserId { get; set; } // Foreign key to User table

        [Required]
        public Guid OrderId { get; set; } // Foreign key to Order table
    }
}