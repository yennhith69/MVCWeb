using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MVCShopping1.Models;

namespace MVCShopping1.Data
{
    public class MVCShopping1Context : DbContext
    {
        public MVCShopping1Context (DbContextOptions<MVCShopping1Context> options)
            : base(options)
        {
        }

        public DbSet<Customers> Customers { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = default!;
        public DbSet<Employee> Employees { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // OrderDetail dùng 2 cột làm khóa chính cùng lúc
            modelBuilder.Entity<OrderDetail>()
                .HasKey(o => new { o.OrderID, o.ProductID });
        }
    }
}
