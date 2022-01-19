using System;
using System.Linq;
using System.Threading.Tasks;
using Atles.Infrastructure.Services;

namespace Atles.Infrastructure.Events
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IServiceProviderWrapper _serviceProvider;

        public EventPublisher(IServiceProviderWrapper serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();

            var tasks = handlers.Select(handler => handler.Handle(@event)).ToList();

            await Task.WhenAll(tasks);
        }
    }
}
