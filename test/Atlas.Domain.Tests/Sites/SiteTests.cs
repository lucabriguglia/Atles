using System;
using Atlas.Domain.Sites;
using AutoFixture;
using NUnit.Framework;

namespace Atlas.Domain.Tests.Sites
{
    [TestFixture]
    public class SiteTests : TestFixtureBase
    {
        [Test]
        public void New()
        {
            const string name = "Default";
            const string title = "My Site";

            var sut = new Site(name, title);

            Assert.AreEqual(name, sut.Name, nameof(sut.Name));
            Assert.AreEqual(title, sut.Title, nameof(sut.Title));
        }

        [Test]
        public void New_passing_id()
        {
            var id = Guid.NewGuid();
            const string name = "Default";
            const string title = "My Site";

            var sut = new Site(id, name, title);

            Assert.AreEqual(id, sut.Id, nameof(sut.Id));
            Assert.AreEqual(name, sut.Name, nameof(sut.Name));
            Assert.AreEqual(title, sut.Title, nameof(sut.Title));
        }

        [Test]
        public void Update_details()
        {
            var sut = Fixture.Create<Site>();

            const string title = "New Title";

            sut.UpdateDetails(title);

            Assert.AreEqual(title, sut.Title, nameof(sut.Title));
        }
    }
}
