using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Core;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Queries;
using Moq;
using NUnit.Framework;

namespace Atles.Infrastructure.Tests
{
    public class DispatcherTests
    {
        [Test]
        public async Task Should_send_command()
        {
            var command = new SampleCommand();
            var @event = new SampleEvent();
            var events = new List<IEvent>{@event};

            var commandSender = new Mock<ICommandSender>();
            commandSender.Setup(x => x.Send(command)).ReturnsAsync(events);

            var queryProcessor = new Mock<IQueryProcessor>();

            var eventPublisher = new Mock<IEventPublisher>();
            eventPublisher.Setup(x => x.Publish(@event)).Returns(Task.CompletedTask);

            var sut = new Dispatcher(commandSender.Object, queryProcessor.Object, eventPublisher.Object);

            await sut.Send(command);

            commandSender.Verify(x => x.Send(command), Times.Once);
        }

        [Test]
        public async Task Should_get_result()
        {
            var query = new SampleQuery();
            var result = new SampleResult();

            var commandSender = new Mock<ICommandSender>();

            var queryProcessor = new Mock<IQueryProcessor>();
            queryProcessor.Setup(x => x.Process(query)).ReturnsAsync(result);

            var eventPublisher = new Mock<IEventPublisher>();

            var sut = new Dispatcher(commandSender.Object, queryProcessor.Object, eventPublisher.Object);

            var actual = await sut.Get(query);

            queryProcessor.Verify(x => x.Process(query), Times.Once);
            Assert.AreEqual(result, actual);
        }

        [Test]
        public async Task Should_publish_event()
        {
            var @event = new SampleEvent();

            var commandSender = new Mock<ICommandSender>();

            var queryProcessor = new Mock<IQueryProcessor>();

            var eventPublisher = new Mock<IEventPublisher>();
            eventPublisher.Setup(x => x.Publish(@event)).Returns(Task.CompletedTask);

            var sut = new Dispatcher(commandSender.Object, queryProcessor.Object, eventPublisher.Object);

            await sut.Publish(@event);

            eventPublisher.Verify(x => x.Publish(@event), Times.Once);
        }
    }
}
