using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Models;
using Atles.Domain.Models.Forums;
using Atles.Domain.Models.Forums.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;

namespace Atles.Domain.Handlers.Forums.Commands
{
    public class UpdateForumHandler : ICommandHandler<UpdateForum>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<UpdateForum> _validator;

        public UpdateForumHandler(AtlesDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<UpdateForum> validator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _validator = validator;
        }

        public async Task Handle(UpdateForum command)
        {
            await _validator.ValidateCommandAsync(command);

            var forum = await _dbContext.Forums
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.Category.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status != ForumStatusType.Deleted);

            if (forum == null)
            {
                throw new DataException($"Forum with Id {command.Id} not found.");
            }

            var originalCategoryId = forum.CategoryId;

            if (originalCategoryId != command.CategoryId)
            {
                forum.Category.DecreaseTopicsCount(forum.TopicsCount);
                forum.Category.DecreaseRepliesCount(forum.RepliesCount);

                var newCategory = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == command.CategoryId);
                newCategory.IncreaseTopicsCount(forum.TopicsCount);
                newCategory.IncreaseRepliesCount(forum.RepliesCount);

                await ReorderForumsInCategory(originalCategoryId, command.Id, command.SiteId, command.UserId);

                var newCategoryForumsCount = await _dbContext.Forums
                    .Where(x => x.CategoryId == command.CategoryId && x.Status != ForumStatusType.Deleted)
                    .CountAsync();

                forum.Reorder(newCategoryForumsCount + 1);
            }

            forum.UpdateDetails(command.CategoryId, command.Name, command.Slug, command.Description, command.PermissionSetId);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Updated,
                typeof(Forum),
                forum.Id,
                new
                {
                    forum.Name,
                    forum.Slug,
                    forum.Description,
                    forum.CategoryId,
                    forum.PermissionSetId
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(command.Id));
            _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));
        }

        private async Task ReorderForumsInCategory(Guid categoryId, Guid forumIdToExclude, Guid siteId, Guid memberId)
        {
            var forums = await _dbContext.Forums
                .Where(x =>
                    x.CategoryId == categoryId &&
                    x.Id != forumIdToExclude &&
                    x.Status != ForumStatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            for (int i = 0; i < forums.Count; i++)
            {
                forums[i].Reorder(i + 1);
                _dbContext.Events.Add(new Event(siteId,
                    memberId,
                    EventType.Reordered,
                    typeof(Forum),
                    forums[i].Id,
                    new
                    {
                        forums[i].SortOrder
                    }));
            }
        }
    }
}
