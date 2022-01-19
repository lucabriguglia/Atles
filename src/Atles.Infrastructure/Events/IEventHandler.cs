using System.Threading.Tasks;

namespace Atles.Infrastructure.Events
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        Task Handle(TEvent @event);
    }
}
