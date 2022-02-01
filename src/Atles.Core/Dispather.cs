using System.Linq;
using System.Threading.Tasks;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Queries;

namespace Atles.Core
{
    public class Dispatcher : IDispatcher
    {
        private readonly ICommandSender _commandSender;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IEventPublisher _eventPublisher;

        public Dispatcher(ICommandSender commandSender, IQueryProcessor queryProcessor, IEventPublisher eventPublisher)
        {
            _commandSender = commandSender;
            _queryProcessor = queryProcessor;
            _eventPublisher = eventPublisher;
        }

        public async Task Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            var events = await _commandSender.Send(command);
            var tasks = events.Select(@event => _eventPublisher.Publish(@event)).ToList();
            await Task.WhenAll(tasks);
        }

        public async Task<TResult> Get<TResult>(IQuery<TResult> query)
        {
            return await _queryProcessor.Process(query);
        }

        public async Task Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            await _eventPublisher.Publish(@event);
        }
    }
}
