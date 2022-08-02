using Atles.Domain;
using AutoFixture;
using NUnit.Framework;

namespace Atles.Tests.Unit.Domain
{
    [TestFixture]
    public class UserRankTests : TestFixtureBase
    {
        [Test]
        public void Should_create_a_new_user_rank()
        {
            var siteId = Guid.NewGuid();
            const string name = "Level 4";
            const string description = "Advanced Level";
            const int sortOrder = 4;
            const string badge = "advanced";
            const string role = "Advanced";
            const UserRankStatusType status = UserRankStatusType.Published;
            var rules = new List<UserRankRule>
            {
                new UserRankRule(UserRankRuleType.Answers, "Name", "Description", 10, "badge")
            };

            var sut = new UserRank(siteId, name, description, sortOrder, badge, role, status, rules);

            Assert.AreNotEqual(Guid.Empty, sut.Id);
            Assert.AreEqual(siteId, sut.SiteId);
            Assert.AreEqual(name, sut.Name);
            Assert.AreEqual(description, sut.Description);
            Assert.AreEqual(sortOrder, sut.SortOrder);
            Assert.AreEqual(badge, sut.Badge);
            Assert.AreEqual(role, sut.Role);
            Assert.AreEqual(status, sut.Status);
            Assert.AreEqual(rules[0].Name, sut.UserRankRules.FirstOrDefault().Name);
            Assert.AreEqual(rules[0].Description, sut.UserRankRules.FirstOrDefault().Description);
            Assert.AreEqual(rules[0].Count, sut.UserRankRules.FirstOrDefault().Count);
            Assert.AreEqual(rules[0].Badge, sut.UserRankRules.FirstOrDefault().Badge);
        }

        [Test]
        public void Should_move_up()
        {
            var sut = Fixture.Create<UserRank>();

            var currentSortOrder = sut.SortOrder;

            sut.MoveUp();

            Assert.AreEqual(currentSortOrder - 1, sut.SortOrder);
        }

        [Test]
        public void Should_throws_exception_when_move_up_and_sort_order_is_one()
        {
            var sut = new UserRank(Guid.NewGuid(), "Level 1", "Basic Level", 1, "basic", "Basic", UserRankStatusType.Published, new List<UserRankRule>());

            Assert.Throws<ApplicationException>(() => sut.MoveUp());
        }

        [Test]
        public void Should_move_down()
        {
            var sut = Fixture.Create<UserRank>();

            var currentSortOrder = sut.SortOrder;

            sut.MoveDown();

            Assert.AreEqual(currentSortOrder + 1, sut.SortOrder, nameof(sut.SortOrder));
        }

        [Test]
        public void Delete()
        {
            var sut = Fixture.Create<UserRank>();

            sut.Delete();

            Assert.AreEqual(UserRankStatusType.Deleted, sut.Status);
        }
    }
}
