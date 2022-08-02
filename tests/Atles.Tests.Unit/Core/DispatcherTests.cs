using Atles.Core;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Mapping;
using Atles.Core.Queries;
using Atles.Core.Results.Types;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Core
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
            commandSender.Setup(x => x.Send(command)).ReturnsAsync(new Success(events));

            var queryProcessor = new Mock<IQueryProcessor>();

            var eventPublisher = new Mock<IEventPublisher>();
            eventPublisher.Setup(x => x.Publish(@event)).Returns(Task.CompletedTask);

            var objectFactory = new Mock<IObjectFactory>();
            objectFactory.Setup(x => x.CreateConcreteObject(It.IsAny<object>())).Returns(new SampleEvent());

            var sut = new Dispatcher(commandSender.Object, queryProcessor.Object, eventPublisher.Object, objectFactory.Object);

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

            var objectFactory = new Mock<IObjectFactory>();

            var sut = new Dispatcher(commandSender.Object, queryProcessor.Object, eventPublisher.Object, objectFactory.Object);

            var actual = await sut.Get(query);

            queryProcessor.Verify(x => x.Process(query), Times.Once);
            Assert.AreEqual(result, actual.AsT0);
        }

        [Test]
        public async Task Should_publish_event()
        {
            var @event = new SampleEvent();

            var commandSender = new Mock<ICommandSender>();

            var queryProcessor = new Mock<IQueryProcessor>();

            var eventPublisher = new Mock<IEventPublisher>();
            eventPublisher.Setup(x => x.Publish(@event)).Returns(Task.CompletedTask);

            var objectFactory = new Mock<IObjectFactory>();

            var sut = new Dispatcher(commandSender.Object, queryProcessor.Object, eventPublisher.Object, objectFactory.Object);

            await sut.Publish(@event);

            eventPublisher.Verify(x => x.Publish(@event), Times.Once);
        }
    }
}
