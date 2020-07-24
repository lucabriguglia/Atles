using Atlas.Domain;
using AutoFixture;
using NUnit.Framework;
using System;
using Atlas.Domain.Forums;

namespace Atlas.Tests.Domain.Forums
{
    [TestFixture]
    public class ForumTests : TestFixtureBase
    {
        [Test]
        public void New()
        {
            var forumGroupId = Guid.NewGuid();
            var name = "New Forum";
            var sortOrder = 2;
            var permissionSetId = Guid.NewGuid();

            var sut = new Forum(forumGroupId, name, sortOrder, permissionSetId);

            Assert.AreEqual(forumGroupId, sut.CategoryId, nameof(sut.CategoryId));
            Assert.AreEqual(name, sut.Name, nameof(sut.Name));
            Assert.AreEqual(sortOrder, sut.SortOrder, nameof(sut.SortOrder));
            Assert.AreEqual(permissionSetId, sut.PermissionSetId, nameof(sut.PermissionSetId));
        }

        [Test]
        public void New_passing_id()
        {
            var id = Guid.NewGuid();
            var forumGroupId = Guid.NewGuid();
            var name = "New Forum";
            var sortOrder = 2;
            var permissionSetId = Guid.NewGuid();

            var sut = new Forum(id, forumGroupId, name, sortOrder, permissionSetId);

            Assert.AreEqual(id, sut.Id, nameof(sut.Id));
            Assert.AreEqual(forumGroupId, sut.CategoryId, nameof(sut.CategoryId));
            Assert.AreEqual(name, sut.Name, nameof(sut.Name));
            Assert.AreEqual(sortOrder, sut.SortOrder, nameof(sut.SortOrder));
            Assert.AreEqual(permissionSetId, sut.PermissionSetId, nameof(sut.PermissionSetId));
        }

        [Test]
        public void Update_details()
        {
            var sut = Fixture.Create<Forum>();

            var forumGroupId = Guid.NewGuid();
            var name = "Updated Forum";
            var permissionSetId = Guid.NewGuid();

            sut.UpdateDetails(forumGroupId, name, permissionSetId);

            Assert.AreEqual(forumGroupId, sut.CategoryId, nameof(sut.CategoryId));
            Assert.AreEqual(name, sut.Name, nameof(sut.Name));
            Assert.AreEqual(permissionSetId, sut.PermissionSetId, nameof(sut.PermissionSetId));
        }

        [Test]
        public void Move_up()
        {
            var sut = Fixture.Create<Forum>();

            var currentSortOrder = sut.SortOrder;

            sut.MoveUp();

            Assert.AreEqual(currentSortOrder - 1, sut.SortOrder, nameof(sut.SortOrder));
        }

        [Test]
        public void Move_up_throws_exception_when_sort_order_is_one()
        {
            var sut = new Forum(Guid.NewGuid(), "My Forum", 1);

            Assert.Throws<ApplicationException>(() => sut.MoveUp());
        }

        [Test]
        public void Move_down()
        {
            var sut = Fixture.Create<Forum>();

            var currentSortOrder = sut.SortOrder;

            sut.MoveDown();

            Assert.AreEqual(currentSortOrder + 1, sut.SortOrder, nameof(sut.SortOrder));
        }

        [Test]
        public void Reorder()
        {
            var sut = Fixture.Create<Forum>();

            var sortOrder = 2;

            sut.Reorder(sortOrder);

            Assert.AreEqual(sortOrder, sut.SortOrder, nameof(sut.SortOrder));
        }

        [Test]
        public void Increase_topics_count()
        {
            var sut = Fixture.Create<Forum>();

            var currentCount = sut.TopicsCount;

            sut.IncreaseTopicsCount();

            Assert.AreEqual(currentCount + 1, sut.TopicsCount, nameof(sut.TopicsCount));
        }

        [Test]
        public void Decrease_topics_count()
        {
            var sut = Fixture.Create<Forum>();
            sut.IncreaseTopicsCount();

            var currentCount = sut.TopicsCount;

            sut.DecreaseTopicsCount();

            Assert.AreEqual(currentCount - 1, sut.TopicsCount, nameof(sut.TopicsCount));
        }

        [Test]
        public void Decrease_topics_count_less_than_zero()
        {
            var sut = Fixture.Create<Forum>();

            sut.DecreaseTopicsCount();

            Assert.AreEqual(0, sut.TopicsCount, nameof(sut.TopicsCount));
        }

        [Test]
        public void Increase_replies_count()
        {
            var sut = Fixture.Create<Forum>();

            var currentCount = sut.RepliesCount;

            sut.IncreaseRepliesCount();

            Assert.AreEqual(currentCount + 1, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void Decrease_replies_count()
        {
            var sut = Fixture.Create<Forum>();
            sut.IncreaseRepliesCount();

            var currentCount = sut.RepliesCount;

            sut.DecreaseRepliesCount();

            Assert.AreEqual(currentCount - 1, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void Decrease_replies_count_less_than_zero()
        {
            var sut = Fixture.Create<Forum>();

            sut.DecreaseRepliesCount();

            Assert.AreEqual(0, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void Delete()
        {
            var sut = Fixture.Create<Forum>();

            sut.Delete();

            Assert.AreEqual(StatusType.Deleted, sut.Status, nameof(sut.Status));
        }
    }
}
