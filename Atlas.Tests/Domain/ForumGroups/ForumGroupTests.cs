using Atlas.Domain;
using AutoFixture;
using NUnit.Framework;
using System;

namespace Atlas.Tests.Domain.ForumGroups
{
    [TestFixture]
    public class ForumGroupTests : TestFixtureBase
    {
        [Test]
        public void New()
        {
            var siteId = Guid.NewGuid();
            var name = "New Forum Group";
            var sortOrder = 2;
            var permissionSetId = Guid.NewGuid();

            var sut = new ForumGroup(siteId, name, sortOrder, permissionSetId);

            Assert.AreEqual(siteId, sut.SiteId, nameof(sut.SiteId));
            Assert.AreEqual(name, sut.Name, nameof(sut.Name));
            Assert.AreEqual(sortOrder, sut.SortOrder, nameof(sut.SortOrder));
            Assert.AreEqual(permissionSetId, sut.PermissionSetId, nameof(sut.PermissionSetId));
        }

        [Test]
        public void UpdateDetails()
        {
            var sut = Fixture.Create<ForumGroup>();

            var name = "Updated Forum Group";
            var permissionSetId = Guid.NewGuid();

            sut.UpdateDetails(name, permissionSetId);

            Assert.AreEqual(name, sut.Name, nameof(sut.Name));
            Assert.AreEqual(permissionSetId, sut.PermissionSetId, nameof(sut.PermissionSetId));
        }

        [Test]
        public void MoveUp()
        {
            var sut = Fixture.Create<ForumGroup>();

            var currentSortOrder = sut.SortOrder;

            sut.MoveUp();

            Assert.AreEqual(currentSortOrder - 1, sut.SortOrder, nameof(sut.SortOrder));
        }

        [Test]
        public void MoveDown()
        {
            var sut = Fixture.Create<ForumGroup>();

            var currentSortOrder = sut.SortOrder;

            sut.MoveDown();

            Assert.AreEqual(currentSortOrder + 1, sut.SortOrder, nameof(sut.SortOrder));
        }

        [Test]
        public void Reorder()
        {
            var sut = Fixture.Create<ForumGroup>();

            var sortOrder = 2;

            sut.Reorder(sortOrder);

            Assert.AreEqual(sortOrder, sut.SortOrder, nameof(sut.SortOrder));
        }

        [Test]
        public void IncreaseTopicsCount()
        {
            var sut = Fixture.Create<ForumGroup>();

            var currentCount = sut.TopicsCount;

            sut.IncreaseTopicsCount();

            Assert.AreEqual(currentCount + 1, sut.TopicsCount, nameof(sut.TopicsCount));
        }

        [Test]
        public void DecreaseTopicsCount()
        {
            var sut = Fixture.Create<ForumGroup>();
            sut.IncreaseTopicsCount();

            var currentCount = sut.TopicsCount;

            sut.DecreaseTopicsCount();

            Assert.AreEqual(currentCount - 1, sut.TopicsCount, nameof(sut.TopicsCount));
        }

        [Test]
        public void DecreaseTopicsCount_LessThanZero()
        {
            var sut = Fixture.Create<ForumGroup>();

            sut.DecreaseTopicsCount();

            Assert.AreEqual(0, sut.TopicsCount, nameof(sut.TopicsCount));
        }

        [Test]
        public void IncreaseRepliesCount()
        {
            var sut = Fixture.Create<ForumGroup>();

            var currentCount = sut.RepliesCount;

            sut.IncreaseRepliesCount();

            Assert.AreEqual(currentCount + 1, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void DecreaseRepliesCount()
        {
            var sut = Fixture.Create<ForumGroup>();
            sut.IncreaseRepliesCount();

            var currentCount = sut.RepliesCount;

            sut.DecreaseRepliesCount();

            Assert.AreEqual(currentCount - 1, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void DecreaseRepliesCount_LessThanZero()
        {
            var sut = Fixture.Create<ForumGroup>();

            sut.DecreaseRepliesCount();

            Assert.AreEqual(0, sut.RepliesCount, nameof(sut.RepliesCount));
        }

        [Test]
        public void Delete()
        {
            var sut = Fixture.Create<ForumGroup>();

            sut.Delete();

            Assert.AreEqual(StatusType.Deleted, sut.Status, nameof(sut.Status));
        }
    }
}
