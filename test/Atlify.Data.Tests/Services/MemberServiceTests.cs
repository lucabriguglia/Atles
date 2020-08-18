using System;
using System.Threading;
using System.Threading.Tasks;
using Atlify.Data.Services;
using Atlify.Domain;
using Atlify.Domain.Members;
using Atlify.Domain.Members.Commands;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atlify.Data.Tests.Services
{
    [TestFixture]
    public class MemberServiceTests : TestFixtureBase
    {
        [Test]
        public async Task Should_create_new_member_and_add_event()
        {
            using (var dbContext = new AtlifyDbContext(Shared.CreateContextOptions()))
            {
                var command = Fixture.Create<CreateMember>();

                var createValidator = new Mock<IValidator<CreateMember>>();
                createValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var updateValidator = new Mock<IValidator<UpdateMember>>();

                var sut = new MemberService(dbContext,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.CreateAsync(command);

                var member = await dbContext.Members.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                createValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(member);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_update_member_and_add_event()
        {
            var options = Shared.CreateContextOptions();
            var member = new Member(Guid.NewGuid(), Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlifyDbContext(options))
            {
                dbContext.Members.Add(member);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlifyDbContext(options))
            {
                var command = Fixture.Build<UpdateMember>()
                        .With(x => x.Id, member.Id)
                        .Create();

                var createValidator = new Mock<IValidator<CreateMember>>();

                var updateValidator = new Mock<IValidator<UpdateMember>>();
                updateValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new MemberService(dbContext,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.UpdateAsync(command);

                var updatedMember = await dbContext.Members.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                updateValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.DisplayName, updatedMember.DisplayName);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_suspend_member_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var memberId = Guid.NewGuid();

            var member = new Member(memberId, Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlifyDbContext(options))
            {
                dbContext.Members.Add(member);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlifyDbContext(options))
            {
                var command = Fixture.Build<SuspendMember>()
                    .With(x => x.Id, member.Id)
                    .Create();

                var createValidator = new Mock<IValidator<CreateMember>>();
                var updateValidator = new Mock<IValidator<UpdateMember>>();

                var sut = new MemberService(dbContext,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.SuspendAsync(command);

                var memberSuspended = await dbContext.Members.FirstOrDefaultAsync(x => x.Id == member.Id);
                var memberEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == member.Id);

                Assert.AreEqual(StatusType.Suspended, memberSuspended.Status);
                Assert.NotNull(memberEvent);
            }
        }

        [Test]
        public async Task Should_reinstate_member_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var memberId = Guid.NewGuid();

            var member = new Member(memberId, Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlifyDbContext(options))
            {
                dbContext.Members.Add(member);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlifyDbContext(options))
            {
                var command = Fixture.Build<ReinstateMember>()
                    .With(x => x.Id, member.Id)
                    .Create();

                var createValidator = new Mock<IValidator<CreateMember>>();
                var updateValidator = new Mock<IValidator<UpdateMember>>();

                var sut = new MemberService(dbContext,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.ReinstateAsync(command);

                var memberResumed = await dbContext.Members.FirstOrDefaultAsync(x => x.Id == member.Id);
                var memberEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == member.Id);

                Assert.AreEqual(StatusType.Active, memberResumed.Status);
                Assert.NotNull(memberEvent);
            }
        }

        [Test]
        public async Task Should_delete_member_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var memberId = Guid.NewGuid();

            var member = new Member(memberId, Guid.NewGuid().ToString(), "me@email.com", "Display Name");

            using (var dbContext = new AtlifyDbContext(options))
            {
                dbContext.Members.Add(member);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlifyDbContext(options))
            {
                var command = Fixture.Build<DeleteMember>()
                        .With(x => x.Id, member.Id)
                        .Create();

                var createValidator = new Mock<IValidator<CreateMember>>();
                var updateValidator = new Mock<IValidator<UpdateMember>>();

                var sut = new MemberService(dbContext,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.DeleteAsync(command);

                var memberDeleted = await dbContext.Members.FirstOrDefaultAsync(x => x.Id == member.Id);
                var memberEvent = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == member.Id);

                Assert.AreEqual(StatusType.Deleted, memberDeleted.Status);
                Assert.NotNull(memberEvent);
            }
        }
    }
}
