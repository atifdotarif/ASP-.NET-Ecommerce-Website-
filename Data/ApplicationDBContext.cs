using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using api.DTOs;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }
        public DbSet<Users> User { get; set; }
        public DbSet<Products> Product { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DeliveryAddress> Addresses { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<ProductWithSellerDTO> ProductWithSellerDTO { get; set; }

        public DbSet<OrderItemWithSellerDto> OrderItemWithSellerDtos { get; set; }
        public DbSet<OrderItemWithBuyerDto> OrderItemWithBuyerDtos { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItemWithSellerDto>().HasNoKey(); // DTO doesn't map to a table directly
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OrderItemWithBuyerDto>().HasNoKey(); // DTO does not have a primary key
            base.OnModelCreating(modelBuilder);
        }






    }
}