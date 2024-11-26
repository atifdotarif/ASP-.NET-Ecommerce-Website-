using System;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class DeliveryAddress
    {
        [Key]
        public Guid AddressId { get; set; }

        [Required]
        [StringLength(100)]
        public string Street { get; set; }=string.Empty;

        [Required]
        [StringLength(50)]
        public string City { get; set; }=string.Empty;

        [Required]
        [StringLength(50)]
        public string State { get; set; }=string.Empty;

        [Required]
        [StringLength(20)]
        public string ZipCode { get; set; }=string.Empty;

        [Required]
        [StringLength(50)]
        public string PhoneNumber { get; set; }=string.Empty;

        [Required]
        public Guid UserId { get; set; } // Foreign key to User table
    }
}
