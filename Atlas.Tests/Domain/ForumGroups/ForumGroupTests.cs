using Atlas.Domain;
using NUnit.Framework;
using System;

namespace Atlas.Tests.Domain.ForumGroups
{
    [TestFixture]
    public class ForumGroupTests
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
    }
}
