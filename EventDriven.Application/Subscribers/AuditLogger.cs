using EventDriven.Application.Abstractions;
using EventDriven.Domain.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventDriven.Application.Subscribers
{
    public class AuditLogger : BackgroundService

    {
        private readonly IEventBus _bus;
        private readonly ILogger<AuditLogger> _logger;
        public AuditLogger(IEventBus bus, ILogger<AuditLogger> logger) => (_bus, _logger) = (bus, logger);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // A real system would fan-out across many event types; we subscribe to all three.
            await foreach (var e in _bus.SubscribeAsync<ProductUpdated>(stoppingToken))
                _logger.LogInformation("[Audit] Product {Name} (id {Id}) updated to quantity {Qty}", e.Name, e.Id, e.Quantity);

            // Note: a Channel<T> is per-type. The gateway below subscribes to the other two.
        }
    }
}
