using EventDriven.Application.Subscribers;
using EventDriven.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEventDrivenInfrastructure(builder.Configuration.GetConnectionString("InventoryDb")!);
builder.Services.AddScoped<EventDriven.Application.ProductService>();

builder.Services.AddHostedService<LowStockNotifier>();
builder.Services.AddHostedService<AuditLogger>();
builder.Services.AddHostedService<DeletedProductReporter>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventDriven.Infrastructure.AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (!await db.Products.AnyAsync())
    {
        db.Products.AddRange(
            new EventDriven.Domain.Product { Name = "Wireless Mouse", Sku = "WM-001", Category = "Accessories", Quantity = 120, UnitPrice = 19.99m, LastUpdatedUtc = DateTime.UtcNow },
            new EventDriven.Domain.Product { Name = "Mechanical Keyboard", Sku = "KB-002", Category = "Accessories", Quantity = 75, UnitPrice = 89.50m, LastUpdatedUtc = DateTime.UtcNow },
            new EventDriven.Domain.Product { Name = "27\" Monitor", Sku = "MN-003", Category = "Displays", Quantity = 30, UnitPrice = 249.00m, LastUpdatedUtc = DateTime.UtcNow });
        await db.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
