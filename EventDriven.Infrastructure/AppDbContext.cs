using EventDriven.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventDriven.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Name).IsRequired().HasMaxLength(200);
                e.Property(p => p.Sku).IsRequired().HasMaxLength(50);
                e.Property(p => p.Category).HasMaxLength(100);
                e.Property(p => p.UnitPrice).HasPrecision(18, 2);
            });
            base.OnModelCreating(modelBuilder);
        }

    }
}
