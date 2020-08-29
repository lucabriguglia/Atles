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

                var member = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                createValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(member);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_update_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();
            var member = new User(Guid.NewGuid(), Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Users.Add(member);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<UpdateUser>()
                        .With(x => x.Id, member.Id)
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

                var updatedMember = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                updateValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.DisplayName, updatedMember.DisplayName);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_suspend_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var memberId = Guid.NewGuid();

            var member = new User(memberId, Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Users.Add(member);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<SuspendUser>()
                    .With(x => x.Id, member.Id)
                    .Create();

                var createValidator = new Mock<IValidator<CreateUser>>();
                var updateValidator = new Mock<IValidator<UpdateUser>>();

                var sut = new UserService(dbContext,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.SuspendAsync(command);

                var memberSuspended = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == member.Id);
                var memberEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == member.Id);

                Assert.AreEqual(StatusType.Suspended, memberSuspended.Status);
                Assert.NotNull(memberEvent);
            }
        }

        [Test]
        public async Task Should_reinstate_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var memberId = Guid.NewGuid();

            var member = new User(memberId, Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Users.Add(member);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<ReinstateUser>()
                    .With(x => x.Id, member.Id)
                    .Create();

                var createValidator = new Mock<IValidator<CreateUser>>();
                var updateValidator = new Mock<IValidator<UpdateUser>>();

                var sut = new UserService(dbContext,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.ReinstateAsync(command);

                var memberResumed = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == member.Id);
                var memberEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == member.Id);

                Assert.AreEqual(StatusType.Active, memberResumed.Status);
                Assert.NotNull(memberEvent);
            }
        }

        [Test]
        public async Task Should_delete_user_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var memberId = Guid.NewGuid();

            var member = new User(memberId, Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.Users.Add(member);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<DeleteUser>()
                        .With(x => x.Id, member.Id)
                        .Create();

                var createValidator = new Mock<IValidator<CreateUser>>();
                var updateValidator = new Mock<IValidator<UpdateUser>>();

                var sut = new UserService(dbContext,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.DeleteAsync(command);

                var memberDeleted = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == member.Id);
                var memberEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == member.Id);

                Assert.AreEqual(StatusType.Deleted, memberDeleted.Status);
                Assert.NotNull(memberEvent);
            }
        }
    }
}
