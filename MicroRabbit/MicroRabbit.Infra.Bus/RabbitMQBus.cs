using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;

namespace MicroRabbit.Infra.Bus
{
    public sealed class RabbitMQBus : IEventBus
    {
        private readonly IMediator _mediator;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQBus(IMediator mediator, IServiceScopeFactory serviceScopeFactory)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new List<Type>();
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Publish<T>(T @event) where T : Event
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5673,
                UserName = "test",
                Password = "test"
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var eventName = @event.GetType().Name;

            await channel.QueueDeclareAsync(eventName, exclusive: false, autoDelete: false);
            string message = System.Text.Json.JsonSerializer.Serialize(@event);

            var body = System.Text.Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync("", eventName, new ReadOnlyMemory<byte>(body));
        }

        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        public async Task Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }

            if (!_handlers.ContainsKey(eventName))
            {
                _handlers[eventName] = [];
            }

            if (_handlers[eventName].Any(s => s.GetType() == handlerType))
            {
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            _handlers[eventName].Add(handlerType);

            await StartBasicConsume<T>();
        }

        private async Task StartBasicConsume<T>() where T : Event
        {
            var eventName = typeof(T).Name;
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5673,
                UserName = "test",
                Password = "test"
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(eventName, exclusive: false, autoDelete: false);
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += ConsumerReceivedAsync;

            await channel.BasicConsumeAsync(eventName, true, consumer);
        }

        private async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs ea)
        {
            var message = System.Text.Encoding.UTF8.GetString(ea.Body.Span);
            var eventName = ea.RoutingKey;

            try
            {
                await ProcessEvent(eventName, message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Handle exception (logging, etc.)
                Console.WriteLine($"Error processing event {eventName}: {ex.Message}");
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if(_handlers.ContainsKey(eventName))
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var subscriptions = _handlers[eventName];

                foreach (var subscription in subscriptions)
                {
                    var handler = scope.ServiceProvider.GetRequiredService(subscription) as IEventHandler;
                    if (handler == null)
                    {
                        continue;
                    }
                    var eventType = _eventTypes.FirstOrDefault(t => t.Name == eventName);
                    var @event = System.Text.Json.JsonSerializer.Deserialize(message, eventType);

                    var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("Handle")?.Invoke(handler, new object[] { @event });
                }
            }
        }
    }
}
