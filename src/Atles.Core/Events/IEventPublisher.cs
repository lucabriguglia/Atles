using System.Threading.Tasks;

namespace Atles.Core.Events;

public interface IEventPublisher
{
    Task Publish<TEvent>(TEvent @event) where TEvent : IEvent;
}