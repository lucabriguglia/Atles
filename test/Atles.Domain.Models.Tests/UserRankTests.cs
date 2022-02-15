using System;
using System.Linq;
using AutoFixture;
using NUnit.Framework;

namespace Atles.Domain.Models.Tests
{
    [TestFixture]
    public class UserRankTests : TestFixtureBase
    {
        [Test]
        public void Should_create_a_new_user_rank()
        {
            const string name = "Level 4";
            const string description = "Advanced Level";
            const int sortOrder = 4;
            const string badge = "advanced";
            const string role = "Advanced";

            var sut = new UserRank(name, description, sortOrder, badge, role);

            Assert.AreNotEqual(Guid.Empty, sut.Id);
            Assert.AreEqual(name, sut.Name);
            Assert.AreEqual(description, sut.Description);
            Assert.AreEqual(sortOrder, sut.SortOrder);
            Assert.AreEqual(badge, sut.Badge);
            Assert.AreEqual(role, sut.Role);
        }

        [Test]
        public void Should_create_add_rule()
        {
            var sut = new UserRank("Level 4", "Advanced Level", 4, "advanced", "Advanced");

            const UserRankRuleType type = UserRankRuleType.Topics; 
            const string name = "50 Topics";
            const string description = "Advanced Posters";
            const int count = 50;
            const string badge = "topic50";

            sut.AddRule(type, name, description, count, badge);

            var rule = sut.UserRankRules.FirstOrDefault();

            Assert.NotNull(rule);
            Assert.AreEqual(type, rule.Type);
            Assert.AreEqual(name, rule.Name);
            Assert.AreEqual(description, rule.Description);
            Assert.AreEqual(count, rule.Count);
            Assert.AreEqual(badge, rule.Badge);
        }

        [Test]
        public void Move_up()
        {
            var sut = Fixture.Create<UserRank>();

            var currentSortOrder = sut.SortOrder;

            sut.MoveUp();

            Assert.AreEqual(currentSortOrder - 1, sut.SortOrder);
        }

        [Test]
        public void Move_up_throws_exception_when_sort_order_is_one()
        {
            var sut = Fixture.Build<UserRank>().Create();

            Assert.Throws<ApplicationException>(() => sut.MoveUp());
        }

        [Test]
        public void Move_down()
        {
            var sut = Fixture.Create<UserRank>();

            var currentSortOrder = sut.SortOrder;

            sut.MoveDown();

            Assert.AreEqual(currentSortOrder + 1, sut.SortOrder, nameof(sut.SortOrder));
        }
    }
}
