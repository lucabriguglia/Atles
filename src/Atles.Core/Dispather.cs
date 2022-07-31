using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Mapping;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Core.Results.Types;

namespace Atles.Core;

public class Dispatcher : IDispatcher
{
    private readonly ICommandSender _commandSender;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IEventPublisher _eventPublisher;
    private readonly IObjectFactory _objectFactory;

    public Dispatcher(ICommandSender commandSender, IQueryProcessor queryProcessor, IEventPublisher eventPublisher, IObjectFactory objectFactory)
    {
        _commandSender = commandSender;
        _queryProcessor = queryProcessor;
        _eventPublisher = eventPublisher;
        _objectFactory = objectFactory;
    }

    public async Task<CommandResult> Send<TCommand>(TCommand command) where TCommand : ICommand
    {
        var commandResult = await _commandSender.Send(command);

        return await commandResult.Match(
            async success => await HandleSuccess(success),
            async failure => await HandleFailure(failure)
        );

        async Task<CommandResult> HandleSuccess(Success success)
        {
            var events = success.Events.ToList();

            if (!events.Any())
            {
                return success;
            }

            var tasks = new List<Task>();

            foreach (var @event in events)
            {
                var concreteEvent = _objectFactory.CreateConcreteObject(@event);
                var task = _eventPublisher.Publish(concreteEvent);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            return success;
        }

        async Task<CommandResult> HandleFailure(Failure failure)
        {
            return await Task.FromResult(failure);
        }
    }

    public async Task<QueryResult<TResult>> Get<TResult>(IQuery<TResult> query)
    {
        return await _queryProcessor.Process(query);
    }

    public async Task Publish<TEvent>(TEvent @event) where TEvent : IEvent
    {
        await _eventPublisher.Publish(@event);
    }
}
