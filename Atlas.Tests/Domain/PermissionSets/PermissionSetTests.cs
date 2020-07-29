using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Domain;
using Atlas.Domain.PermissionSets;
using Atlas.Domain.PermissionSets.Commands;
using AutoFixture;
using NUnit.Framework;

namespace Atlas.Tests.Domain.PermissionSets
{
    [TestFixture]
    public class PermissionSetTests : TestFixtureBase
    {
        [Test]
        public void New()
        {
            var siteId = Guid.NewGuid();
            const string name = "New Permission Set";
            var permissions = new List<Permission>();

            var sut = new PermissionSet(siteId, name, new List<PermissionCommand>());

            Assert.AreEqual(siteId, sut.SiteId, nameof(sut.SiteId));
            Assert.AreEqual(name, sut.Name, nameof(sut.Name));
            Assert.AreEqual(permissions, sut.Permissions, nameof(sut.Permissions));
        }

        [Test]
        public void New_passing_id()
        {
            var id = Guid.NewGuid();
            var siteId = Guid.NewGuid();
            const string name = "New Permission Set";
            var permissions = new List<PermissionCommand>();

            var sut = new PermissionSet(id, siteId, name, permissions);

            Assert.AreEqual(id, sut.Id, nameof(sut.Id));
            Assert.AreEqual(siteId, sut.SiteId, nameof(sut.SiteId));
            Assert.AreEqual(name, sut.Name, nameof(sut.Name));
            Assert.AreEqual(permissions, sut.Permissions, nameof(sut.Permissions));
        }

        [Test]
        public void Update_details()
        {
            var sut = Fixture.Create<PermissionSet>();

            const string name = "Updated Permission Set";
            var permissions = new List<PermissionCommand> {new PermissionCommand
            {
                Type = PermissionType.Pin, 
                RoleId = Guid.NewGuid().ToString()
            }};

            sut.UpdateDetails(name, permissions);

            Assert.AreEqual(name, sut.Name, nameof(sut.Name));
            Assert.AreEqual(permissions.FirstOrDefault().RoleId, sut.Permissions.FirstOrDefault().RoleId, nameof(sut.Permissions));
        }

        [Test]
        public void Delete()
        {
            var sut = Fixture.Create<PermissionSet>();

            sut.Delete();

            Assert.AreEqual(StatusType.Deleted, sut.Status, nameof(sut.Status));
        }
    }
}
