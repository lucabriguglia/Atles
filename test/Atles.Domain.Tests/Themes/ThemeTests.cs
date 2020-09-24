using System;
using Atles.Domain.Themes;
using NUnit.Framework;
using AutoFixture;

namespace Atles.Domain.Tests.Themes
{
    [TestFixture]
    public class ThemeTests : TestFixtureBase
    {
        [Test]
        public void New()
        {
            var siteId = Guid.NewGuid();
            const string name = "New Theme";

            var sut = Theme.CreateNew(siteId, name);

            Assert.AreEqual(siteId, sut.SiteId, nameof(sut.SiteId));
            Assert.AreEqual(name, sut.Name, nameof(sut.Name));
            Assert.AreEqual(ThemeStatus.Published, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void New_passing_id()
        {
            var id = Guid.NewGuid();
            var siteId = Guid.NewGuid();
            const string name = "New Theme";

            var sut = Theme.CreateNew(id, siteId, name);

            Assert.AreEqual(id, sut.Id, nameof(sut.Id));
            Assert.AreEqual(siteId, sut.SiteId, nameof(sut.SiteId));
            Assert.AreEqual(name, sut.Name, nameof(sut.Name));
            Assert.AreEqual(ThemeStatus.Published, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Update_details()
        {
            var sut = Fixture.Create<Theme>();

            const string name = "Updated Theme";

            sut.UpdateDetails(name);

            Assert.AreEqual(name, sut.Name, nameof(sut.Name));
        }

        [Test]
        public void Delete()
        {
            var sut = Fixture.Create<Theme>();

            sut.Delete();

            Assert.AreEqual(ThemeStatus.Deleted, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Restore()
        {
            var sut = Fixture.Create<Theme>();

            sut.Restore();

            Assert.AreEqual(ThemeStatus.Published, sut.Status, nameof(sut.Status));
        }
    }
}