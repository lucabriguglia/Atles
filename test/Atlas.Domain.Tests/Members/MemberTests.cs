using System;
using Atlas.Domain.Members;
using AutoFixture;
using NUnit.Framework;

namespace Atlas.Domain.Tests.Members
{
    [TestFixture]
    public class MemberTests : TestFixtureBase
    {
        [Test]
        public void New()
        {
            var userId = Guid.NewGuid().ToString();
            const string email = "abc@def.ghi";
            const string displayName = "Display Name";

            var sut = new Member(userId, email, displayName);

            Assert.AreEqual(userId, sut.UserId, nameof(sut.UserId));
            Assert.AreEqual(email, sut.Email, nameof(sut.Email));
            Assert.AreEqual(displayName, sut.DisplayName, nameof(sut.DisplayName));
            Assert.AreEqual(StatusType.Active, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void New_passing_id()
        {
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            const string email = "abc@def.ghi";
            const string displayName = "Display Name";

            var sut = new Member(id, userId, email, displayName);

            Assert.AreEqual(id, sut.Id, nameof(sut.Id));
            Assert.AreEqual(userId, sut.UserId, nameof(sut.UserId));
            Assert.AreEqual(email, sut.Email, nameof(sut.Email));
            Assert.AreEqual(displayName, sut.DisplayName, nameof(sut.DisplayName));
            Assert.AreEqual(StatusType.Active, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Update_details()
        {
            var sut = Fixture.Create<Member>();

            const string displayName = "New Display Name";

            sut.UpdateDetails(displayName);

            Assert.AreEqual(displayName, sut.DisplayName, nameof(sut.DisplayName));
        }

        [Test]
        public void Increase_topics_count()
        {
            var sut = Fixture.Create<Member>();

            var currentCount = sut.TopicsCount;

            sut.IncreaseTopicsCount();

            Assert.AreEqual(currentCount + 1, sut.TopicsCount, nameof(sut.TopicsCount));
        }

        [Test]
        public void Decrease_topics_count()
        {
            var sut = Fixture.Create<Member>();
            sut.IncreaseTopicsCount();

            var currentCount = sut.TopicsCount;

            sut.DecreaseTopicsCount();

            Assert.AreEqual(currentCount - 1, sut.TopicsCount, nameof(sut.TopicsCount));
        }

        [Test]
        public void Decrease_topics_count_less_than_zero()
        {
            var sut = Fixture.Create<Member>();

            sut.DecreaseTopicsCount();

            Assert.AreEqual(0, sut.TopicsCount, nameof(sut.TopicsCount));
        }

        [Test]
        public void Increase_replies_count()
        {
            var sut = Fixture.Create<Member>();

            var currentCount = sut.RepliesCount;

            sut.IncreaseRepliesCount();

            Assert.AreEqual(currentCount + 1, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void Decrease_replies_count()
        {
            var sut = Fixture.Create<Member>();
            sut.IncreaseRepliesCount();

            var currentCount = sut.RepliesCount;

            sut.DecreaseRepliesCount();

            Assert.AreEqual(currentCount - 1, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void Decrease_replies_count_less_than_zero()
        {
            var sut = Fixture.Create<Member>();

            sut.DecreaseRepliesCount();

            Assert.AreEqual(0, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void Delete()
        {
            var sut = Fixture.Create<Member>();

            sut.Delete();

            Assert.AreEqual(StatusType.Deleted, sut.Status, nameof(sut.Status));
        }
    }
}
