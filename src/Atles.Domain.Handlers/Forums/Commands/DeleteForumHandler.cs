using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Models;
using Atles.Domain.Models.Forums;
using Atles.Domain.Models.Forums.Commands;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;

namespace Atles.Domain.Handlers.Forums.Commands
{
    public class DeleteForumHandler : ICommandHandler<DeleteForum>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public DeleteForumHandler(AtlesDbContext dbContext,
            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task Handle(DeleteForum command)
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
            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Deleted,
                typeof(Forum),
                forum.Id));

            await ReorderForumsInCategory(forum.CategoryId, command.Id, command.SiteId, command.UserId);

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(forum.Id));
            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
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
