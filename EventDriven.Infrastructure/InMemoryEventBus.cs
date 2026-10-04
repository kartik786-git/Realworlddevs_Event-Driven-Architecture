using EventDriven.Application.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace EventDriven.Infrastructure
{
    public class InMemoryEventBus : IEventBus
    {
        private sealed class ChannelSet
        {
            public Channel<object> ChannelInstance = Channel.CreateUnbounded<object>(
                new UnboundedChannelOptions { SingleReader = false, SingleWriter = false });
        }

        private readonly ConcurrentDictionary<Type, ChannelSet> _channels = new();

        private ChannelSet For(Type type) => _channels.GetOrAdd(type, _ => new ChannelSet());

        public ValueTask PublishAsync<T>(T @event, CancellationToken ct = default) where T : class
        {
            var channel = For(typeof(T)).ChannelInstance;
            return channel.Writer.WriteAsync(@event, ct);
        }

        public IAsyncEnumerable<T> SubscribeAsync<T>(CancellationToken ct = default) where T : class
        {
            var channel = For(typeof(T)).ChannelInstance;
            return channel.Reader.ReadAllAsync(ct).Select(x => (T)x);
        }
    }
}
