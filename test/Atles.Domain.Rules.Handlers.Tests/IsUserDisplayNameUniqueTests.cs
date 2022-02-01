using Atles.Data;
using Atles.Domain.Models;
using NUnit.Framework;

namespace Atles.Domain.Rules.Handlers.Tests
{
    [TestFixture]
    public class IsUserDisplayNameUniqueTests : TestFixtureBase
    {
        [Test]
        public async Task Should_return_true_when_display_name_is_unique()
        {
            var options = Shared.CreateContextOptions();

            using (var dbContext = new AtlesDbContext(options))
            {
                var user = new User(Guid.NewGuid().ToString(), "me@email.com", "Display Name");
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new IsUserDisplayNameUniqueHandler(dbContext);
                var actual = await sut.Handle(new IsUserDisplayNameUnique { DisplayName = "Blah blah" });

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public async Task Should_return_true_when_display_name_is_unique_for_existing_member()
        {
            var options = Shared.CreateContextOptions();

            using (var dbContext = new AtlesDbContext(options))
            {
                var user1 = new User(Guid.NewGuid().ToString(), "me1@email.com", "User 1");
                var user2 = new User(Guid.NewGuid().ToString(), "me2@email.com", "User 2");
                dbContext.Users.Add(user1);
                dbContext.Users.Add(user2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var sut = new IsUserDisplayNameUniqueHandler(dbContext);
                var actual = await sut.Handle(new IsUserDisplayNameUnique { DisplayName = "User 3", Id = Guid.NewGuid() });

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
