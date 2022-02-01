using System;
using Atles.Domain.Models.Subscriptions;
using NUnit.Framework;

namespace Atles.Domain.Models.Tests
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

            var sut = new Subscription(userId, targetId, type);

            Assert.AreEqual(userId, sut.UserId);
            Assert.AreEqual(targetId, sut.TargetId);
            Assert.AreEqual(type, sut.Type);
        }
    }
}
