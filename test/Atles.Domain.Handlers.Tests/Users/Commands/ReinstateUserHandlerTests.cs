using System;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Handlers.Users.Commands;
using Atles.Domain.Models.Users;
using Atles.Domain.Models.Users.Commands;
using AutoFixture;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.Users.Commands
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
                    .With(x => x.Id, user.Id)
                    .Create();

                var createValidator = new Mock<IValidator<CreateUser>>();
                var updateValidator = new Mock<IValidator<UpdateUser>>();

                var sut = new ReinstateUserHandler(dbContext);

                await sut.Handle(command);

                var userReinstated = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                var userEvent = await dbContext.HistoryItems.FirstOrDefaultAsync(x => x.TargetId == user.Id);

                Assert.AreEqual(UserStatusType.Active, userReinstated.Status);
                Assert.NotNull(userEvent);
            }
        }
    }
}
