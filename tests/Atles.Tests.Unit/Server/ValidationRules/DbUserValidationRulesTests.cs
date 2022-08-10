using Atles.Data;
using Atles.Domain;
using Atles.Server.ValidationRules;
using NUnit.Framework;

namespace Atles.Tests.Unit.Server.ValidationRules;

[TestFixture]
public class DbUserValidationRulesTests : TestFixtureBase
{
    [Test]
    public async Task Should_return_true_when_display_name_is_unique()
    {
        var options = Shared.CreateContextOptions();

        await using (var dbContext = new AtlesDbContext(options))
        {
            var user = new User(Guid.NewGuid().ToString(), "me@email.com", "Display Name");
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new DbUserValidationRules(dbContext);
            var actual = await sut.IsUserDisplayNameUnique(Guid.Empty, "Blah blah");

            Assert.IsTrue(actual);
        }
    }

    [Test]
    public async Task Should_return_true_when_display_name_is_unique_for_existing_member()
    {
        var options = Shared.CreateContextOptions();

        await using (var dbContext = new AtlesDbContext(options))
        {
            var user1 = new User(Guid.NewGuid().ToString(), "me1@email.com", "User 1");
            var user2 = new User(Guid.NewGuid().ToString(), "me2@email.com", "User 2");
            dbContext.Users.Add(user1);
            dbContext.Users.Add(user2);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new DbUserValidationRules(dbContext);
            var actual = await sut.IsUserDisplayNameUnique(Guid.NewGuid(), "User 3");

            Assert.IsTrue(actual);
        }
    }

    [Test]
    public async Task Should_return_false_when_display_name_is_not_unique()
    {
        var options = Shared.CreateContextOptions();
        var userId = Guid.NewGuid();
        var displayName = "Display Name";

        await using (var dbContext = new AtlesDbContext(options))
        {
            var user = new User(userId, Guid.NewGuid().ToString(), "me@email.com", displayName);
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new DbUserValidationRules(dbContext);
            var actual = await sut.IsUserDisplayNameUnique(userId, displayName);

            Assert.IsFalse(actual);
        }
    }

    [Test]
    public async Task Should_return_false_when_display_name_is_not_unique_for_existing_member()
    {
        var options = Shared.CreateContextOptions();
        var userId = Guid.NewGuid();

        await using (var dbContext = new AtlesDbContext(options))
        {
            var user1 = new User(Guid.NewGuid().ToString(), "me@email.com", "User 1");
            var user2 = new User(userId, Guid.NewGuid().ToString(), "me@email.com", "User 2");
            dbContext.Users.Add(user1);
            dbContext.Users.Add(user2);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AtlesDbContext(options))
        {
            var sut = new DbUserValidationRules(dbContext);
            var actual = await sut.IsUserDisplayNameUnique(userId, "User 1");

            Assert.IsFalse(actual);
        }
    }
}
