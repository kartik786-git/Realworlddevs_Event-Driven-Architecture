using System;
using System.Collections.Generic;
using System.Text;

namespace EventDriven.Application.Abstractions
{
    public interface IEventBus
    {
        ValueTask PublishAsync<T>(T @event, CancellationToken ct = default) where T : class;
        IAsyncEnumerable<T> SubscribeAsync<T>(CancellationToken ct = default) where T : class;
    }
}
