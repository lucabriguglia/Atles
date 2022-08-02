using Atles.Domain;
using AutoFixture;
using NUnit.Framework;

namespace Atles.Tests.Unit.Domain
{
    [TestFixture]
    public class ReplyTests : TestFixtureBase
    {
        [Test]
        public void Should_create_new_reply()
        {
            var topicId = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            const string content = "Blah blah blah...";
            const PostStatusType status = PostStatusType.Draft;

            var sut = Post.CreateReply(topicId, forumId, userId, content, status);

            Assert.AreEqual(topicId, sut.TopicId, nameof(sut.TopicId));
            Assert.AreEqual(forumId, sut.ForumId, nameof(sut.ForumId));
            Assert.AreEqual(userId, sut.CreatedBy, nameof(sut.CreatedBy));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Should_create_new_reply_with_id()
        {
            var id = Guid.NewGuid();
            var topicId = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            const string content = "Blah blah blah...";
            const PostStatusType status = PostStatusType.Draft;

            var sut = Post.CreateReply(id, topicId, forumId, userId, content, status);

            Assert.AreEqual(id, sut.Id, nameof(sut.Id));
            Assert.AreEqual(topicId, sut.TopicId, nameof(sut.TopicId));
            Assert.AreEqual(forumId, sut.ForumId, nameof(sut.ForumId));
            Assert.AreEqual(userId, sut.CreatedBy, nameof(sut.CreatedBy));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Should_update_reply_details()
        {
            var sut = Fixture.Create<Post>();

            var userId = Guid.NewGuid();
            const string content = "Blah blah blah...";
            const PostStatusType status = PostStatusType.Draft;

            sut.UpdateDetails(userId, content, status);

            Assert.AreEqual(userId, sut.ModifiedBy, nameof(sut.ModifiedBy));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_set_reply_as_answer(bool isAnswer)
        {
            var sut = Post.CreateReply(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Content", PostStatusType.Published);

            sut.SetAsAnswer(isAnswer);

            Assert.AreEqual(isAnswer, sut.IsAnswer, nameof(sut.IsAnswer));
        }

        [Test]
        public void Should_delete_post()
        {
            var sut = Fixture.Create<Post>();

            sut.Delete();

            Assert.AreEqual(PostStatusType.Deleted, sut.Status, nameof(sut.Status));
        }
    }
}
