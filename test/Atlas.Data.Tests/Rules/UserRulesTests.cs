using System;
using System.Threading.Tasks;
using Atlas.Data.Rules;
using Atlas.Domain.Users;
using NUnit.Framework;

namespace Atlas.Data.Tests.Rules
{
    [TestFixture]
    public class UserRulesTests : TestFixtureBase
    {
        [Test]
        [Ignore("")]
        public async Task Should_return_true_when_display_name_is_unique()
        {
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var sut = new UserRules(dbContext);
                var actual = await sut.IsDisplayNameUniqueAsync("Display Name");

                Assert.IsTrue(actual);
            }
        }

        [Test]
        [Ignore("")]
        public async Task Should_return_true_when_display_name_is_unique_for_existing_member()
        {
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var sut = new UserRules(dbContext);
                var actual = await sut.IsDisplayNameUniqueAsync("Display Name", Guid.NewGuid());

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_display_name_is_not_unique()
        {
            var options = Shared.CreateContextOptions();
            var displayName = "Display Name";

            using (var dbContext = new AtlasDbContext(options))
            {
                var member = new User(Guid.NewGuid().ToString(), "me@email.com", displayName);
                dbContext.Users.Add(member);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var sut = new UserRules(dbContext);
                var actual = await sut.IsDisplayNameUniqueAsync(displayName);

                Assert.IsFalse(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_display_name_is_not_unique_for_existing_member()
        {
            var options = Shared.CreateContextOptions();
            var memberId = Guid.NewGuid();

            using (var dbContext = new AtlasDbContext(options))
            {
                var user1 = new User(Guid.NewGuid().ToString(), "me@email.com", "User 1");
                var user2 = new User(memberId, Guid.NewGuid().ToString(), "me@email.com", "User 2");
                dbContext.Users.Add(user1);
                dbContext.Users.Add(user2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var sut = new UserRules(dbContext);
                var actual = await sut.IsDisplayNameUniqueAsync("User 1", memberId);

                Assert.IsFalse(actual);
            }
        }
    }
}
