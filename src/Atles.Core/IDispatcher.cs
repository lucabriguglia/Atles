using System.Threading.Tasks;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Queries;

namespace Atles.Core
{
    public interface IDispatcher
    {
        Task Send<TCommand>(TCommand command) where TCommand : ICommand;
        Task<TResult> Get<TResult>(IQuery<TResult> query);
        Task Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
