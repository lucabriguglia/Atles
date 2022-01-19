using System.Threading.Tasks;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;
using Atles.Infrastructure.Queries;

namespace Atles.Infrastructure
{
    public interface IDispatcher
    {
        Task Send<TCommand>(TCommand command) where TCommand : ICommand;
        Task<TResult> Get<TResult>(IQuery<TResult> query);
        Task Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
