using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results.Types;
using Atles.Core.Services;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Core
{
    public class CommandSenderTests
    {
        [Test]
        public void Should_throw_argument_null_exception_when_sending_null_command()
        {
            var serviceProvider = new Mock<IServiceProviderWrapper>();
            var sut = new CommandSender(serviceProvider.Object);
            Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.Send(default(SampleCommand)!));
        }

        [Test]
        public void Should_throw_exception_when_handler_not_found()
        {
            var serviceProvider = new Mock<IServiceProviderWrapper>();
            var sut = new CommandSender(serviceProvider.Object);
            Assert.ThrowsAsync<Exception>(async () => await sut.Send(new SampleCommand()));
        }

        [Test]
        public async Task Should_handle_command()
        {
            var command = new SampleCommand();

            var handler = new Mock<ICommandHandler<SampleCommand>>();
            handler.Setup(x => x.Handle(command)).ReturnsAsync(new Success(new List<IEvent>()));

            var serviceProvider = new Mock<IServiceProviderWrapper>();
            serviceProvider.Setup(x => x.GetService<ICommandHandler<SampleCommand>>()).Returns(handler.Object);

            var sut = new CommandSender(serviceProvider.Object);

            await sut.Send(command);

            handler.Verify(x => x.Handle(command), Times.Once);
        }
    }
}
