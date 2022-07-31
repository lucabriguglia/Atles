using System.Threading.Tasks;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Queries;
using Atles.Core.Results;

namespace Atles.Core;

public interface IDispatcher
{
    Task<CommandResult> Send<TCommand>(TCommand command) where TCommand : ICommand;
    Task<QueryResult<TResult>> Get<TResult>(IQuery<TResult> query);
    Task Publish<TEvent>(TEvent @event) where TEvent : IEvent;
}
