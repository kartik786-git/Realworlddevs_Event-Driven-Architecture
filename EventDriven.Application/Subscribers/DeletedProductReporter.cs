using EventDriven.Application.Abstractions;
using EventDriven.Domain.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventDriven.Application.Subscribers
{
    public class DeletedProductReporter : BackgroundService
    {
        private readonly IEventBus _bus;
        private readonly ILogger<DeletedProductReporter> _logger;
        public DeletedProductReporter(IEventBus bus, ILogger<DeletedProductReporter> logger) => (_bus, _logger) = (bus, logger);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var e in _bus.SubscribeAsync<ProductDeleted>(stoppingToken))
                _logger.LogInformation("[Report] Product '{Name}' (id {Id}) was removed from inventory", e.Name, e.Id);
        }
    }
}
