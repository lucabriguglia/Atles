using Atles.Domain;
using AutoFixture;
using NUnit.Framework;

namespace Atles.Tests.Unit.Domain
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
            Assert.AreEqual("Default", sut.PublicTheme, nameof(sut.PublicTheme));
            Assert.AreEqual("public.css", sut.PublicCss, nameof(sut.PublicCss));
            Assert.AreEqual("Default", sut.AdminTheme, nameof(sut.AdminTheme));
            Assert.AreEqual("admin.css", sut.AdminCss, nameof(sut.AdminCss));
            Assert.AreEqual("en", sut.Language, nameof(sut.Language));
            Assert.AreEqual("Privacy statement...", sut.Privacy, nameof(sut.Privacy));
            Assert.AreEqual("Terms of service...", sut.Terms, nameof(sut.Terms));
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
            Assert.AreEqual("Default", sut.PublicTheme, nameof(sut.PublicTheme));
            Assert.AreEqual("public.css", sut.PublicCss, nameof(sut.PublicCss));
            Assert.AreEqual("Default", sut.AdminTheme, nameof(sut.AdminTheme));
            Assert.AreEqual("admin.css", sut.AdminCss, nameof(sut.AdminCss));
            Assert.AreEqual("en", sut.Language, nameof(sut.Language));
            Assert.AreEqual("Privacy statement...", sut.Privacy, nameof(sut.Privacy));
            Assert.AreEqual("Terms of service...", sut.Terms, nameof(sut.Terms));
        }

        [Test]
        public void Update_details()
        {
            var sut = Fixture.Create<Site>();

            const string title = "New Title";
            const string theme = "New Theme";
            const string css = "New CSS";
            const string language = "it";
            const string privacy = "Privacy";
            const string terms = "Terms";
            const string headScript = "<script>...blah...</script>";

            sut.UpdateDetails(title, theme, css, language, privacy, terms, headScript);

            Assert.AreEqual(title, sut.Title, nameof(sut.Title));
            Assert.AreEqual(theme, sut.PublicTheme, nameof(sut.PublicTheme));
            Assert.AreEqual(css, sut.PublicCss, nameof(sut.PublicCss));
            Assert.AreEqual(language, sut.Language, nameof(sut.Language));
            Assert.AreEqual(privacy, sut.Privacy, nameof(sut.Privacy));
            Assert.AreEqual(terms, sut.Terms, nameof(sut.Terms));
            Assert.AreEqual(headScript, sut.HeadScript, nameof(sut.HeadScript));
        }
    }
}
