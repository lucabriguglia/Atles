using System;
using System.Threading;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Handlers.Users.Commands;
using Atles.Domain.Users;
using Atles.Domain.Users.Commands;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.Users.Commands
{
    [TestFixture]
    public class UpdateUserHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_update_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();
            var user = new User(Guid.NewGuid(), Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<UpdateUser>()
                        .With(x => x.Id, user.Id)
                        .Create();

                var validator = new Mock<IValidator<UpdateUser>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new UpdateUserHandler(dbContext, validator.Object);

                await sut.Handle(command);

                var updatedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.DisplayName, updatedUser.DisplayName);
                Assert.NotNull(@event);
            }
        }
    }
}
