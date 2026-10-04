using EventDriven.Application.Abstractions;
using EventDriven.Application.Dto;
using EventDriven.Domain;
using EventDriven.Domain.Events;
using EventDriven.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventDriven.Application
{
    public class ProductService
    {
        private readonly AppDbContext _db;
        private readonly IEventBus _bus;
        public ProductService(AppDbContext db, IEventBus bus) => (_db, _bus) = (db, bus);

        public async Task<List<ProductDto>> GetAllAsync() =>
       await _db.Products.AsNoTracking().OrderBy(p => p.Name)
           .Select(p => ToDto(p)).ToListAsync();


        public async Task<ProductDto?> GetByIdAsync(int id) =>
        await _db.Products.AsNoTracking().Where(p => p.Id == id)
            .Select(p => ToDto(p)).FirstOrDefaultAsync();

        public async Task<ProductDto> CreateAsync(ProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Sku = dto.Sku,
                Category = dto.Category,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                LastUpdatedUtc = DateTime.UtcNow
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            // Notify the rest of the system without coupling to specific subscribers.
            await _bus.PublishAsync(new ProductCreated(product.Id, product.Name, product.Quantity));
            return ToDto(product);
        }

        public async Task<bool> UpdateAsync(int id, ProductDto dto)
        {
            var existing = await _db.Products.FindAsync(id);
            if (existing is null) return false;

            existing.Name = dto.Name;
            existing.Sku = dto.Sku;
            existing.Category = dto.Category;
            existing.Quantity = dto.Quantity;
            existing.UnitPrice = dto.UnitPrice;
            existing.LastUpdatedUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _bus.PublishAsync(new ProductUpdated(existing.Id, existing.Name, existing.Quantity));
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Products.FindAsync(id);
            if (existing is null) return false;

            var name = existing.Name;
            _db.Products.Remove(existing);
            await _db.SaveChangesAsync();

            await _bus.PublishAsync(new ProductDeleted(id, name));
            return true;
        }

        private static ProductDto ToDto(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Sku = p.Sku,
            Category = p.Category,
            Quantity = p.Quantity,
            UnitPrice = p.UnitPrice,
            LastUpdatedUtc = p.LastUpdatedUtc
        };
    }
}
