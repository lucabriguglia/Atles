using Atles.Domain;
using AutoFixture;
using NUnit.Framework;

namespace Atles.Tests.Unit.Domain
{
    [TestFixture]
    public class TopicTests : TestFixtureBase
    {
        [Test]
        public void Should_create_new_topic()
        {
            var forumId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            const string title = "New Topic";
            const string slug = "my-topic-slug";
            const string content = "Blah blah blah...";
            const PostStatusType status = PostStatusType.Draft;

            var sut = Post.CreateTopic(forumId, userId, title, slug, content, status);

            Assert.AreEqual(forumId, sut.ForumId, nameof(sut.ForumId));
            Assert.AreEqual(userId, sut.CreatedBy, nameof(sut.CreatedBy));
            Assert.AreEqual(title, sut.Title, nameof(sut.Title));
            Assert.AreEqual(slug, sut.Slug, nameof(sut.Slug));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Should_create_new_topic_with_id()
        {
            var id = Guid.NewGuid();
            var forumId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            const string title = "New Topic";
            const string slug = "my-topic-slug";
            const string content = "Blah blah blah...";
            const PostStatusType status = PostStatusType.Draft;

            var sut = Post.CreateTopic(id, forumId, userId, title, slug, content, status);

            Assert.AreEqual(id, sut.Id, nameof(sut.Id));
            Assert.AreEqual(forumId, sut.ForumId, nameof(sut.ForumId));
            Assert.AreEqual(userId, sut.CreatedBy, nameof(sut.CreatedBy));
            Assert.AreEqual(title, sut.Title, nameof(sut.Title));
            Assert.AreEqual(slug, sut.Slug, nameof(sut.Slug));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Should_update_topic_details()
        {
            var sut = Fixture.Create<Post>();

            var userId = Guid.NewGuid();
            const string title = "Updated Topic";
            const string slug = "my-topic-slug";
            const string content = "Blah blah blah...";
            const PostStatusType status = PostStatusType.Draft;

            sut.UpdateDetails(userId, title, slug, content, status);

            Assert.AreEqual(userId, sut.ModifiedBy, nameof(sut.ModifiedBy));
            Assert.AreEqual(title, sut.Title, nameof(sut.Title));
            Assert.AreEqual(slug, sut.Slug, nameof(sut.Slug));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Should_update_last_reply()
        {
            var sut = Post.CreateTopic(Guid.NewGuid(), Guid.NewGuid(), "Title", "slug", "Content", PostStatusType.Published);

            var lastReplyId = Guid.NewGuid();

            sut.UpdateLastReply(lastReplyId);

            Assert.AreEqual(lastReplyId, sut.LastReplyId, nameof(sut.LastReplyId));
        }

        [Test]
        public void Should_not_update_last_reply_if_it_is_not_a_topic()
        {
            var sut = Post.CreateReply(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Content", PostStatusType.Published);

            sut.UpdateLastReply(Guid.NewGuid());

            Assert.IsNull(sut.LastReplyId, nameof(sut.LastReplyId));
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

        [TestCase(true)]
        [TestCase(false)]
        public void Pin(bool pinned)
        {
            var sut = Fixture.Create<Post>();

            sut.Pin(pinned);

            Assert.AreEqual(pinned, sut.Pinned, nameof(sut.Pinned));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Lock(bool locked)
        {
            var sut = Fixture.Create<Post>();

            sut.Lock(locked);

            Assert.AreEqual(locked, sut.Locked, nameof(sut.Locked));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SetAsAnswered(bool hasAnswer)
        {
            var sut = Fixture.Create<Post>();

            sut.SetAsAnswered(hasAnswer);

            Assert.AreEqual(hasAnswer, sut.HasAnswer, nameof(sut.HasAnswer));
        }
    }
}
