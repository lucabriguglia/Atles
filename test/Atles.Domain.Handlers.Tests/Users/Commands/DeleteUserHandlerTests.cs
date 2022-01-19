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
    public class DeleteUserHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_delete_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var userId = Guid.NewGuid();
            var identityUserId = Guid.NewGuid();

            var user = new User(userId, identityUserId.ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<DeleteUser>()
                        .With(x => x.Id, user.Id)
                        .With(x => x.IdentityUserId, user.IdentityUserId)
                        .Create();

                var createValidator = new Mock<IValidator<CreateUser>>();
                var updateValidator = new Mock<IValidator<UpdateUser>>();

                var sut = new DeleteUserHandler(dbContext);

                await sut.Handle(command);

                var userDeleted = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                var userEvent = await dbContext.HistoryItems.FirstOrDefaultAsync(x => x.TargetId == user.Id);

                Assert.AreEqual(UserStatusType.Deleted, userDeleted.Status);
                Assert.NotNull(userEvent);
            }
        }
    }
}
