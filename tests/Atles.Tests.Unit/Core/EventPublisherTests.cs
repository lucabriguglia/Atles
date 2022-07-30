using Atles.Core.Events;
using Atles.Core.Services;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Core
{
    [TestFixture]
    public class EventPublisherTests
    {
        [Test]
        public void Should_throw_exception_when_event_is_null()
        {
            var serviceProvider = new Mock<IServiceProviderWrapper>();

            var sut = new EventPublisher(serviceProvider.Object);

            Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.Publish(default(SampleEvent)!));
        }

        [Test]
        public async Task Should_publish_event()
        {
            var sampleEvent = new SampleEvent();

            var eventHandler1 = new Mock<IEventHandler<SampleEvent>>();
            eventHandler1 .Setup(x => x.Handle(sampleEvent));

            var eventHandler2 = new Mock<IEventHandler<SampleEvent>>();
            eventHandler2 .Setup(x => x.Handle(sampleEvent));

            var serviceProvider = new Mock<IServiceProviderWrapper>();
            serviceProvider
                .Setup(x => x.GetServices<IEventHandler<SampleEvent>>())
                .Returns(new List<IEventHandler<SampleEvent>> { eventHandler1.Object, eventHandler2.Object });

            var sut = new EventPublisher(serviceProvider.Object);

            await sut.Publish(sampleEvent);

            eventHandler1.Verify(x => x.Handle(sampleEvent), Times.Once);
            eventHandler2.Verify(x => x.Handle(sampleEvent), Times.Once);
        }
    }
}