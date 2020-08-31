using System;
using Atlas.Domain.Posts;
using AutoFixture;
using NUnit.Framework;

namespace Atlas.Domain.Tests.Replies
{
    [TestFixture]
    public class ReplyTests : TestFixtureBase
    {
        [Test]
        public void New()
        {
            var topicId = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            const string content = "Blah blah blah...";
            const StatusType status = StatusType.Draft;

            var sut = Post.CreateReply(topicId, forumId, userId, content, status);

            Assert.AreEqual(topicId, sut.TopicId, nameof(sut.TopicId));
            Assert.AreEqual(forumId, sut.ForumId, nameof(sut.ForumId));
            Assert.AreEqual(userId, sut.CreatedBy, nameof(sut.CreatedBy));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void New_passing_id()
        {
            var id = Guid.NewGuid();
            var topicId = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            const string content = "Blah blah blah...";
            const StatusType status = StatusType.Draft;

            var sut = Post.CreateReply(id, topicId, forumId, userId, content, status);

            Assert.AreEqual(id, sut.Id, nameof(sut.Id));
            Assert.AreEqual(topicId, sut.TopicId, nameof(sut.TopicId));
            Assert.AreEqual(forumId, sut.ForumId, nameof(sut.ForumId));
            Assert.AreEqual(userId, sut.CreatedBy, nameof(sut.CreatedBy));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Update_details()
        {
            var sut = Fixture.Create<Post>();

            var userId = Guid.NewGuid();
            const string content = "Blah blah blah...";
            const StatusType status = StatusType.Draft;

            sut.UpdateDetails(userId, content, status);

            Assert.AreEqual(userId, sut.ModifiedBy, nameof(sut.ModifiedBy));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SetAsAnswer(bool isAnswer)
        {
            var sut = Post.CreateReply(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Content", StatusType.Active);

            sut.SetAsAnswer(isAnswer);

            Assert.AreEqual(isAnswer, sut.IsAnswer, nameof(sut.IsAnswer));
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
