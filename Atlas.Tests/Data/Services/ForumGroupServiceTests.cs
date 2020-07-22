using Atlas.Data;
using Atlas.Data.Caching;
using Atlas.Data.Services;
using Atlas.Domain;
using Atlas.Domain.ForumGroups.Commands;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Atlas.Tests.Data.Services
{
    [TestFixture]
    public class ForumGroupServiceTests : TestFixtureBase
    {
        [Test]
        public async Task Should_create_new_forum_group_and_add_event()
        {
            using (var dbContext = new AtlasDbContext(Shared.CreateContextOptions()))
            {
                var command = Fixture.Create<CreateForumGroup>();

                var cacheManager = new Mock<ICacheManager>();

                var createValidator = new Mock<IValidator<CreateForumGroup>>();
                createValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var updateValidator = new Mock<IValidator<UpdateForumGroup>>();

                var sut = new ForumGroupService(dbContext,
                    cacheManager.Object, 
                    createValidator.Object, 
                    updateValidator.Object);

                await sut.CreateAsync(command);

                var forumGroup = await dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                createValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(forumGroup);
                Assert.AreEqual(1, forumGroup.SortOrder);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_update_forum_group_and_add_event()
        {
            var options = Shared.CreateContextOptions();
            var forumGroup = Fixture.Create<ForumGroup>();

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.ForumGroups.Add(forumGroup);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<UpdateForumGroup>()
                        .With(x => x.Id, forumGroup.Id)
                        .With(x => x.SiteId, forumGroup.SiteId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var createValidator = new Mock<IValidator<CreateForumGroup>>();

                var updateValidator = new Mock<IValidator<UpdateForumGroup>>();
                updateValidator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new ForumGroupService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.UpdateAsync(command);

                var updatedForumGroup = await dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                updateValidator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.Name, updatedForumGroup.Name);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_move_forum_group_up_and_add_events()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();

            var forumGroup1 = new ForumGroup(siteId, "Forum Group 1", 1);
            var forumGroup2 = new ForumGroup(siteId, "Forum Group 2", 2);

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.ForumGroups.Add(forumGroup1);
                dbContext.ForumGroups.Add(forumGroup2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = new MoveForumGroup
                {
                    Id = forumGroup2.Id,
                    Direction = Direction.Up,
                    SiteId = siteId
                };

                var cacheManager = new Mock<ICacheManager>();
                var createValidator = new Mock<IValidator<CreateForumGroup>>();
                var updateValidator = new Mock<IValidator<UpdateForumGroup>>();

                var sut = new ForumGroupService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.MoveAsync(command);

                var updatedForumGroup1 = await dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == forumGroup1.Id);
                var updatedForumGroup2 = await dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == forumGroup2.Id);

                var event1 = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forumGroup1.Id);
                var event2 = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forumGroup2.Id);

                Assert.AreEqual(forumGroup2.SortOrder, updatedForumGroup1.SortOrder);
                Assert.AreEqual(forumGroup1.SortOrder, updatedForumGroup2.SortOrder);
                Assert.NotNull(event1);
                Assert.NotNull(event2);
            }
        }

        [Test]
        public async Task Should_move_forum_group_down_and_add_events()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();

            var forumGroup1 = new ForumGroup(siteId, "Forum Group 1", 1);
            var forumGroup2 = new ForumGroup(siteId, "Forum Group 2", 2);

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.ForumGroups.Add(forumGroup1);
                dbContext.ForumGroups.Add(forumGroup2);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = new MoveForumGroup
                {
                    Id = forumGroup1.Id,
                    Direction = Direction.Down,
                    SiteId = siteId
                };

                var cacheManager = new Mock<ICacheManager>();
                var createValidator = new Mock<IValidator<CreateForumGroup>>();
                var updateValidator = new Mock<IValidator<UpdateForumGroup>>();

                var sut = new ForumGroupService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.MoveAsync(command);

                var updatedForumGroup1 = await dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == forumGroup1.Id);
                var updatedForumGroup2 = await dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == forumGroup2.Id);

                var event1 = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forumGroup1.Id);
                var event2 = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forumGroup2.Id);

                Assert.AreEqual(forumGroup2.SortOrder, updatedForumGroup1.SortOrder);
                Assert.AreEqual(forumGroup1.SortOrder, updatedForumGroup2.SortOrder);
                Assert.NotNull(event1);
                Assert.NotNull(event2);
            }
        }

        [Test]
        public async Task Should_delete_forum_group_and_reorder_other_froum_groups_and_add_event()
        {
            var options = Shared.CreateContextOptions();

            var siteId = Guid.NewGuid();

            var forumGroup1 = new ForumGroup(siteId, "Forum Group 1", 1);
            var forumGroup2 = new ForumGroup(siteId, "Forum Group 2", 2);
            var forumGroup3 = new ForumGroup(siteId, "Forum Group 3", 3);
            var forumGroup4 = new ForumGroup(siteId, "Forum Group 4", 4);

            var forum1 = new Forum(forumGroup2.Id, "Forum 1", 1);
            var forum2 = new Forum(forumGroup2.Id, "Forum 2", 2);

            using (var dbContext = new AtlasDbContext(options))
            {
                dbContext.ForumGroups.Add(forumGroup1);
                dbContext.ForumGroups.Add(forumGroup2);
                dbContext.ForumGroups.Add(forumGroup3);
                dbContext.ForumGroups.Add(forumGroup4);

                dbContext.Forums.Add(forum1);
                dbContext.Forums.Add(forum2);

                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlasDbContext(options))
            {
                var command = Fixture.Build<DeleteForumGroup>()
                        .With(x => x.Id, forumGroup2.Id)
                        .With(x => x.SiteId, siteId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();
                var createValidator = new Mock<IValidator<CreateForumGroup>>();
                var updateValidator = new Mock<IValidator<UpdateForumGroup>>();

                var sut = new ForumGroupService(dbContext,
                    cacheManager.Object,
                    createValidator.Object,
                    updateValidator.Object);

                await sut.DeleteAsync(command);

                var forumGroup2Deleted = await dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == command.Id);
                var forumGroup3Reordered = await dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == forumGroup3.Id);
                var forumGroup4Reordered = await dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == forumGroup4.Id);

                var forum1Deleted = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum1.Id);
                var forum2Deleted = await dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forum2.Id);

                var forumGroup1Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forumGroup1.Id);
                var forumGroup2Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);
                var forumGroup3Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forumGroup3.Id);
                var forumGroup4Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forumGroup4.Id);

                var forum1Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forum1.Id);
                var forum2Event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == forum2.Id);

                Assert.AreEqual(StatusType.Deleted, forumGroup2Deleted.Status);
                Assert.AreEqual(forumGroup2.SortOrder, forumGroup3Reordered.SortOrder);
                Assert.AreEqual(forumGroup3.SortOrder, forumGroup4Reordered.SortOrder);
                Assert.AreEqual(StatusType.Deleted, forum1Deleted.Status);
                Assert.AreEqual(StatusType.Deleted, forum2Deleted.Status);
                Assert.NotNull(forumGroup1Event);
                Assert.NotNull(forumGroup2Event);
                Assert.NotNull(forumGroup3Event);
                Assert.NotNull(forumGroup4Event);
                Assert.NotNull(forum1Event);
                Assert.NotNull(forum2Event);
            }
        }
    }
}
