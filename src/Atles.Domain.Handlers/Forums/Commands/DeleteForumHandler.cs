using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Models;
using Atles.Domain.Models.Forums;
using Atles.Domain.Models.Forums.Commands;
using Atles.Domain.Models.Forums.Events;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Handlers.Forums.Commands
{
    public class DeleteForumHandler : ICommandHandler<DeleteForum>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public DeleteForumHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<IEnumerable<IEvent>> Handle(DeleteForum command)
        {
            var forum = await _dbContext.Forums
                .FirstOrDefaultAsync(x =>
                    x.Category.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status != ForumStatusType.Deleted);

            if (forum == null)
            {
                throw new DataException($"Forum with Id {command.Id} not found.");
            }

            forum.Delete();

            var @event = new ForumDeleted
            {
                TargetId = forum.Id,
                TargetType = nameof(Forum),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await ReorderForumsInCategory(forum.CategoryId, command.Id, command.SiteId, command.UserId);

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(forum.Id));
            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
            _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));
        }

        private async Task ReorderForumsInCategory(Guid categoryId, Guid forumIdToExclude, Guid siteId, Guid userId)
        {
            var forums = await _dbContext.Forums
                .Where(x =>
                    x.CategoryId == categoryId &&
                    x.Id != forumIdToExclude &&
                    x.Status != ForumStatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            for (var i = 0; i < forums.Count; i++)
            {
                forums[i].Reorder(i + 1);

                var @event = new ForumMoved
                {
                    SortOrder = forums[i].SortOrder,
                    TargetId = forums[i].Id,
                    TargetType = nameof(Forum),
                    SiteId = siteId,
                    UserId = userId
                };

                _dbContext.Events.Add(@event.ToDbEntity());
            }
        }
    }
}
