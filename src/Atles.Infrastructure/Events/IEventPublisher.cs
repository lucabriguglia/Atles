using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atles.Infrastructure.Events
{
    public interface IEventPublisher
    {
        Task Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
