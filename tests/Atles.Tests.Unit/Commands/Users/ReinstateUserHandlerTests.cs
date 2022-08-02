using Atles.Commands.Handlers.Users;
using Atles.Commands.Users;
using Atles.Data;
using Atles.Domain;
using AutoFixture;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Commands.Users
{
    [TestFixture]
    public class ReinstateUserHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_reinstate_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var userId = Guid.NewGuid();

            var user = new User(userId, Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<ReinstateUser>()
                    .With(x => x.ReinstateUserId, user.Id)
                    .Create();

                var createValidator = new Mock<IValidator<CreateUser>>();
                var updateValidator = new Mock<IValidator<UpdateUser>>();

                var sut = new ReinstateUserHandler(dbContext);

                await sut.Handle(command);

                var userReinstated = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                var userEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == user.Id);

                Assert.AreEqual(UserStatusType.Active, userReinstated.Status);
                Assert.NotNull(userEvent);
            }
        }
    }
}
