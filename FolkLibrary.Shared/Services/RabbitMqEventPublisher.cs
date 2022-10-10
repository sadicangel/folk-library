using EasyNetQ;
using EasyNetQ.Topology;
using FolkLibrary.Events;
using FolkLibrary.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FolkLibrary.Services;

internal sealed class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
    private readonly IAdvancedBus _bus;
    private readonly Exchange _exchange;

    public RabbitMqEventPublisher(IConfiguration configuration)
    {
        _bus = RabbitHutch.CreateBus(configuration.GetConnectionString("RabbitMq"), services => services.Register(typeof(ISerializer), typeof(SystemTextJsonSerializer))).Advanced;
        _bus.ExchangeDeclarePassive(configuration.GetSection("RabbitMq:Topic").Value);
        _exchange = _bus.ExchangeDeclare(configuration.GetSection("RabbitMq:Topic").Value, "topic");
    }

    public void Dispose() => _bus.Dispose();

    public void Publish<T>(DomainEvent<T> @event) => _bus.Publish(_exchange, @event.Type, mandatory: false, @event);

    public Task PublishAsync<T>(DomainEvent<T> @event, CancellationToken cancellationToken = default) => _bus.PublishAsync(@_exchange, @event.Type, mandatory: false, @event, cancellationToken);

    private sealed class SystemTextJsonSerializer : ISerializer
    {
        private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        public IMemoryOwner<byte> MessageToBytes(Type messageType, object message)
        {
            var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(message, messageType, _options);
            return new NoMemoryOwner(bytes);
        }

        public object? BytesToMessage(Type messageType, in ReadOnlyMemory<byte> bytes)
        {
            return System.Text.Json.JsonSerializer.Deserialize(bytes.Span, messageType, _options);
        }

        private sealed class NoMemoryOwner : IMemoryOwner<byte>
        {
            private readonly byte[] _bytes;

            public NoMemoryOwner(byte[] bytes) => _bytes = bytes;

            public void Dispose()
            {
            }

            public Memory<byte> Memory => new(_bytes);
        }
    }
}
