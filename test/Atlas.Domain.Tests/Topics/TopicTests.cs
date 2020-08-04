using System;
using Atlas.Domain.Posts;
using AutoFixture;
using NUnit.Framework;

namespace Atlas.Domain.Tests.Topics
{
    [TestFixture]
    public class TopicTests : TestFixtureBase
    {
        [Test]
        public void New()
        {
            var forumId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            const string title = "New Topic";
            const string content = "Blah blah blah...";
            const StatusType status = StatusType.Draft;

            var sut = Post.CreateTopic(forumId, memberId, title, content, status);

            Assert.AreEqual(forumId, sut.ForumId, nameof(sut.ForumId));
            Assert.AreEqual(memberId, sut.MemberId, nameof(sut.MemberId));
            Assert.AreEqual(title, sut.Title, nameof(sut.Title));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void New_passing_id()
        {
            var id = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            const string title = "New Topic";
            const string content = "Blah blah blah...";
            const StatusType status = StatusType.Draft;

            var sut = Post.CreateTopic(id, forumId, memberId, title, content, status);

            Assert.AreEqual(id, sut.Id, nameof(sut.Id));
            Assert.AreEqual(forumId, sut.ForumId, nameof(sut.ForumId));
            Assert.AreEqual(memberId, sut.MemberId, nameof(sut.MemberId));
            Assert.AreEqual(title, sut.Title, nameof(sut.Title));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Update_details()
        {
            var sut = Fixture.Create<Post>();

            const string title = "Updated Topic";
            const string content = "Blah blah blah...";
            const StatusType status = StatusType.Draft;

            sut.UpdateDetails(title, content, status);

            Assert.AreEqual(title, sut.Title, nameof(sut.Title));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Increase_replies_count()
        {
            var sut = Fixture.Create<Post>();

            var currentCount = sut.RepliesCount;

            sut.IncreaseRepliesCount();

            Assert.AreEqual(currentCount + 1, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void Decrease_replies_count()
        {
            var sut = Fixture.Create<Post>();
            sut.IncreaseRepliesCount();

            var currentCount = sut.RepliesCount;

            sut.DecreaseRepliesCount();

            Assert.AreEqual(currentCount - 1, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void Decrease_replies_count_less_than_zero()
        {
            var sut = Fixture.Create<Post>();

            sut.DecreaseRepliesCount();

            Assert.AreEqual(0, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void Delete()
        {
            var sut = Fixture.Create<Post>();

            sut.Delete();

            Assert.AreEqual(StatusType.Deleted, sut.Status, nameof(sut.Status));
        }
    }
}
