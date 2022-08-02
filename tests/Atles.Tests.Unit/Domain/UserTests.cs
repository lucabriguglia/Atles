using Atles.Domain;
using AutoFixture;
using NUnit.Framework;

namespace Atles.Tests.Unit.Domain
{
    [TestFixture]
    public class UserTests : TestFixtureBase
    {
        [Test]
        public void New()
        {
            var identityUserId = Guid.NewGuid().ToString();
            const string email = "abc@def.ghi";
            const string displayName = "Display Name";

            var sut = new User(identityUserId, email, displayName);

            Assert.AreEqual(identityUserId, sut.IdentityUserId, nameof(sut.IdentityUserId));
            Assert.AreEqual(email, sut.Email, nameof(sut.Email));
            Assert.AreEqual(displayName, sut.DisplayName, nameof(sut.DisplayName));
            Assert.AreEqual(UserStatusType.Pending, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void New_passing_id()
        {
            var id = Guid.NewGuid();
            var identityUserId = Guid.NewGuid().ToString();
            const string email = "abc@def.ghi";
            const string displayName = "Display Name";

            var sut = new User(id, identityUserId, email, displayName);

            Assert.AreEqual(id, sut.Id, nameof(sut.Id));
            Assert.AreEqual(identityUserId, sut.IdentityUserId, nameof(sut.IdentityUserId));
            Assert.AreEqual(email, sut.Email, nameof(sut.Email));
            Assert.AreEqual(displayName, sut.DisplayName, nameof(sut.DisplayName));
            Assert.AreEqual(UserStatusType.Pending, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Update_details()
        {
            var sut = Fixture.Create<User>();

            const string displayName = "New Display Name";

            sut.UpdateDetails(displayName);

            Assert.AreEqual(displayName, sut.DisplayName, nameof(sut.DisplayName));
        }

        [Test]
        public void Should_update_email()
        {
            var sut = Fixture.Create<User>();

            const string email = "new@email.com";

            sut.UpdateEmail(email);

            Assert.AreEqual(email, sut.Email, nameof(sut.Email));
        }

        [Test]
        public void Increase_topics_count()
        {
            var sut = Fixture.Create<User>();

            var currentCount = sut.TopicsCount;

            sut.IncreaseTopicsCount();

            Assert.AreEqual(currentCount + 1, sut.TopicsCount, nameof(sut.TopicsCount));
        }

        [Test]
        public void Decrease_topics_count()
        {
            var sut = Fixture.Create<User>();
            sut.IncreaseTopicsCount();

            var currentCount = sut.TopicsCount;

            sut.DecreaseTopicsCount();

            Assert.AreEqual(currentCount - 1, sut.TopicsCount, nameof(sut.TopicsCount));
        }

        [Test]
        public void Decrease_topics_count_less_than_zero()
        {
            var sut = Fixture.Create<User>();

            sut.DecreaseTopicsCount();

            Assert.AreEqual(0, sut.TopicsCount, nameof(sut.TopicsCount));
        }

        [Test]
        public void Increase_replies_count()
        {
            var sut = Fixture.Create<User>();

            var currentCount = sut.RepliesCount;

            sut.IncreaseRepliesCount();

            Assert.AreEqual(currentCount + 1, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void Decrease_replies_count()
        {
            var sut = Fixture.Create<User>();
            sut.IncreaseRepliesCount();

            var currentCount = sut.RepliesCount;

            sut.DecreaseRepliesCount();

            Assert.AreEqual(currentCount - 1, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void Decrease_replies_count_less_than_zero()
        {
            var sut = Fixture.Create<User>();

            sut.DecreaseRepliesCount();

            Assert.AreEqual(0, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void Increase_answers_count()
        {
            var sut = Fixture.Create<User>();

            var currentCount = sut.AnswersCount;

            sut.IncreaseAnswersCount();

            Assert.AreEqual(currentCount + 1, sut.AnswersCount, nameof(sut.AnswersCount));
        }

        [Test]
        public void Decrease_answers_count()
        {
            var sut = Fixture.Create<User>();
            sut.IncreaseAnswersCount();

            var currentCount = sut.AnswersCount;

            sut.DecreaseAnswersCount();

            Assert.AreEqual(currentCount - 1, sut.AnswersCount, nameof(sut.AnswersCount));
        }

        [Test]
        public void Decrease_answers_count_less_than_zero()
        {
            var sut = Fixture.Create<User>();

            sut.DecreaseAnswersCount();

            Assert.AreEqual(0, sut.AnswersCount, nameof(sut.AnswersCount));
        }

        [Test]
        public void Delete()
        {
            var sut = Fixture.Create<User>();

            sut.Delete();

            Assert.AreEqual(UserStatusType.Deleted, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Suspend()
        {
            var sut = Fixture.Create<User>();

            sut.Suspend();

            Assert.AreEqual(UserStatusType.Suspended, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Reinstate()
        {
            var sut = Fixture.Create<User>();

            sut.Reinstate();

            Assert.AreEqual(UserStatusType.Active, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Confirm()
        {
            var sut = Fixture.Create<User>();

            sut.Confirm();

            Assert.AreEqual(UserStatusType.Active, sut.Status, nameof(sut.Status));
        }
    }
}
