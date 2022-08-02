using Atles.Domain;
using NUnit.Framework;

namespace Atles.Tests.Unit.Domain
{
    [TestFixture]
    public class SubscriptionTests : TestFixtureBase
    {
        [Test]
        public void Should_create_a_new_subscription()
        {
            var userId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            const SubscriptionType type = SubscriptionType.Forum;

            var sut = new Subscription(userId, type, targetId);

            Assert.AreEqual(userId, sut.UserId);
            Assert.AreEqual(targetId, sut.ItemId);
            Assert.AreEqual(type, sut.Type);
        }
    }
}
