using EventDriven.Application.Abstractions;
using EventDriven.Domain.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventDriven.Application.Subscribers
{
    public class LowStockNotifier : BackgroundService
    {
        private readonly IEventBus _bus;
        private readonly ILogger<LowStockNotifier> _logger;
        public LowStockNotifier(IEventBus bus, ILogger<LowStockNotifier> logger) => (_bus, _logger) = (bus, logger);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var e in _bus.SubscribeAsync<ProductCreated>(stoppingToken))
            {
                if (e.Quantity < 50)
                    _logger.LogWarning("[LowStockNotifier] Product {Name} created with low stock: {Qty}", e.Name, e.Quantity);
                else
                    _logger.LogInformation("[LowStockNotifier] Product {Name} created with stock {Qty}", e.Name, e.Quantity);
            }
        }
    }
}
