using Atles.Core.Queries;
using Atles.Core.Results.Types;
using Atles.Core.Services;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Core
{
    public class QueryProcessorTests
    {
        [Test]
        public async Task Should_throw_argument_null_exception_when_sending_null_query()
        {
            var serviceProvider = new Mock<IServiceProviderWrapper>();
            var sut = new QueryProcessor(serviceProvider.Object);
            var actual = await sut.Process<SampleQuery>(null);
            Assert.AreEqual(typeof(Failure), actual.AsT1.GetType());
        }

        [Test]
        public async Task Should_throw_exception_when_handler_not_found()
        {
            var serviceProvider = new Mock<IServiceProviderWrapper>();
            var sut = new QueryProcessor(serviceProvider.Object);
            var actual = await sut.Process<SampleQuery>(null);
            Assert.AreEqual(typeof(Failure), actual.AsT1.GetType());
        }

        [Test]
        public async Task Should_handle_query()
        {
            var query = new SampleQuery();
            var result = new SampleResult();

            var handler = new Mock<IQueryHandler<SampleQuery, SampleResult>>();
            handler.Setup(x => x.Handle(query)).ReturnsAsync(result);

            var serviceProvider = new Mock<IServiceProviderWrapper>();
            serviceProvider.Setup(x => x.GetService<IQueryHandler<SampleQuery, SampleResult>>()).Returns(handler.Object);

            var sut = new QueryProcessor(serviceProvider.Object);

            var actual = await sut.Process(query);

            Assert.AreEqual(result, actual.AsT0);
        }
    }
}
