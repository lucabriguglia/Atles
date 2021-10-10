using System;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Categories.Rules;
using Atles.Domain.Handlers.Users.Rules;
using Atles.Domain.Users;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.Users.Rules
{
    [TestFixture]
    public class IsUserDisplayNameUniqueTests : TestFixtureBase
    {
        [Test]
        [Ignore("")]
        public async Task Should_return_true_when_display_name_is_unique()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new IsUserDisplayNameUniqueHandler(dbContext);
                var actual = await sut.Handle(new IsUserDisplayNameUnique { DisplayName = "Display Name" });

                Assert.IsTrue(actual);
            }
        }

        [Test]
        [Ignore("")]
        public async Task Should_return_true_when_display_name_is_unique_for_existing_member()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var sut = new IsUserDisplayNameUniqueHandler(dbContext);
                var actual = await sut.Handle(new IsUserDisplayNameUnique { DisplayName = "Display Name", Id = Guid.NewGuid() });

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_display_name_is_not_unique()
        {
            var options = Shared.CreateContextOptions();
            var displayName = "Display Name";

            using (var dbContext = new AtlesDbContext(options))
            {
                var user = new User(Guid.NewGuid().ToString(), "me@email.com", displayName);
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new IsUserDisplayNameUniqueHandler(dbContext);
                var actual = await sut.Handle(new IsUserDisplayNameUnique { DisplayName = displayName });

                Assert.IsFalse(actual);
            }
        }

        [Test]
        public async Task Should_return_false_when_display_name_is_not_unique_for_existing_member()
        {
            var options = Shared.CreateContextOptions();
            var userId = Guid.NewGuid();

            using (var dbContext = new AtlesDbContext(options))
            {
                var user1 = new User(Guid.NewGuid().ToString(), "me@email.com", "User 1");
                var user2 = new User(userId, Guid.NewGuid().ToString(), "me@email.com", "User 2");
                dbContext.Users.Add(user1);
                dbContext.Users.Add(user2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new IsUserDisplayNameUniqueHandler(dbContext);
                var actual = await sut.Handle(new IsUserDisplayNameUnique { DisplayName = "User 1", Id = userId });

                Assert.IsFalse(actual);
            }
        }
    }
}
