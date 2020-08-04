using System;
using Atlas.Domain.Replies;
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
            var memberId = Guid.NewGuid();
            const string content = "Blah blah blah...";
            const StatusType status = StatusType.Draft;

            var sut = new Reply(topicId, memberId, content, status);

            Assert.AreEqual(topicId, sut.TopicId, nameof(sut.TopicId));
            Assert.AreEqual(memberId, sut.MemberId, nameof(sut.MemberId));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void New_passing_id()
        {
            var id = Guid.NewGuid();
            var topicId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            const string content = "Blah blah blah...";
            const StatusType status = StatusType.Draft;

            var sut = new Reply(id, topicId, memberId, content, status);

            Assert.AreEqual(id, sut.Id, nameof(sut.Id));
            Assert.AreEqual(topicId, sut.TopicId, nameof(sut.TopicId));
            Assert.AreEqual(memberId, sut.MemberId, nameof(sut.MemberId));
            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Update_details()
        {
            var sut = Fixture.Create<Reply>();

            const string content = "Blah blah blah...";
            const StatusType status = StatusType.Draft;

            sut.UpdateDetails(content, status);

            Assert.AreEqual(content, sut.Content, nameof(sut.Content));
            Assert.AreEqual(status, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Delete()
        {
            var sut = Fixture.Create<Reply>();

            sut.Delete();

            Assert.AreEqual(StatusType.Deleted, sut.Status, nameof(sut.Status));
        }
    }
}
