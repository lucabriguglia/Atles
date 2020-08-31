using System;
using System.Threading;
using System.Threading.Tasks;
using Atlas.Data.Services;
using Atlas.Domain;
using Atlas.Domain.Users;
using Atlas.Domain.Users.Commands;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atlas.Data.Tests.Services
{
    [TestFixture]
    public class UserServiceTests : TestFixtureBase
    {
        [Test]
        public async Task Should_create_new_user_and_add_event()
        {
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var command = Fixture.Create<CreateUser>();

                var createValidator = new Mock<IValidator<CreateUser>>();
                createValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var updateValidator = new Mock<IValidator<UpdateUser>>();

                var sut = new UserService(dbContext,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.CreateAsync(command);

                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                createValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(user);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_update_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();
            var user = new User(Guid.NewGuid(), Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<UpdateUser>()
                        .With(x => x.Id, user.Id)
                        .Create();

                var createValidator = new Mock<IValidator<CreateUser>>();

                var updateValidator = new Mock<IValidator<UpdateUser>>();
                updateValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new UserService(dbContext,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.UpdateAsync(command);

                var updatedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                updateValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.DisplayName, updatedUser.DisplayName);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_suspend_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var userId = Guid.NewGuid();

            var user = new User(userId, Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<SuspendUser>()
                    .With(x => x.Id, user.Id)
                    .Create();

                var createValidator = new Mock<IValidator<CreateUser>>();
                var updateValidator = new Mock<IValidator<UpdateUser>>();

                var sut = new UserService(dbContext,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.SuspendAsync(command);

                var userSuspended = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                var userEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == user.Id);

                Assert.AreEqual(StatusType.Suspended, userSuspended.Status);
                Assert.NotNull(userEvent);
            }
        }

        [Test]
        public async Task Should_reinstate_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var userId = Guid.NewGuid();

            var user = new User(userId, Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<ReinstateUser>()
                    .With(x => x.Id, user.Id)
                    .Create();

                var createValidator = new Mock<IValidator<CreateUser>>();
                var updateValidator = new Mock<IValidator<UpdateUser>>();

                var sut = new UserService(dbContext,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.ReinstateAsync(command);

                var userReinstated = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                var userEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == user.Id);

                Assert.AreEqual(StatusType.Active, userReinstated.Status);
                Assert.NotNull(userEvent);
            }
        }

        [Test]
        public async Task Should_delete_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var userId = Guid.NewGuid();

            var user = new User(userId, Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<DeleteUser>()
                        .With(x => x.Id, user.Id)
                        .Create();

                var createValidator = new Mock<IValidator<CreateUser>>();
                var updateValidator = new Mock<IValidator<UpdateUser>>();

                var sut = new UserService(dbContext,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.DeleteAsync(command);

                var userDeleted = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                var userEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == user.Id);

                Assert.AreEqual(StatusType.Deleted, userDeleted.Status);
                Assert.NotNull(userEvent);
            }
        }
    }
}
