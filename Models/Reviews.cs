using System;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Review
    {
        [Key]
        public Guid ReviewId { get; set; }

        [Required]
        public Guid UserId { get; set; } // Foreign key to User table

        [Required]
        public Guid ProductId { get; set; } // Foreign key to Product table

        [Required]
        [MaxLength(500)]
        public string Comments { get; set; }=string.Empty;

        [Required]
        [Range(1, 5)]
        public int Stars { get; set; }
    }
}
