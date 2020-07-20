using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.ForumGroups;
using Atlas.Domain.ForumGroups.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Services
{
    public class ForumGroupService : IForumGroupService
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<CreateForumGroup> _createValidator;
        private readonly IValidator<UpdateForumGroup> _updateValidator;

        public ForumGroupService(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<CreateForumGroup> createValidator,
            IValidator<UpdateForumGroup> updateValidator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task CreateAsync(CreateForumGroup command)
        {
            await _createValidator.ValidateAndThrowAsync(command);

            var forumGroupsCount = await _dbContext.ForumGroups
                .Where(x => x.SiteId == command.SiteId && x.Status != StatusType.Deleted)
                .CountAsync();

            var sortOrder = forumGroupsCount + 1;

            var forumGroup = new ForumGroup(command.SiteId,
                command.Name,
                sortOrder,
                command.PermissionSetId);

            _dbContext.ForumGroups.Add(forumGroup);
            _dbContext.Events.Add(new Event(nameof(ForumGroup), EventType.Created, forumGroup.Id, command.MemberId, new
            {
                forumGroup.SiteId,
                forumGroup.Name,
                forumGroup.SortOrder,
                forumGroup.PermissionSetId
            }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.ForumGroups(forumGroup.SiteId));
        }

        public async Task UpdateAsync(UpdateForumGroup command)
        {
            await _updateValidator.ValidateAndThrowAsync(command);

            var forumGroup = await _dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == command.Id && x.Status != StatusType.Deleted);

            if (forumGroup == null)
            {
                throw new DataException($"Forum Group with Id {command.Id} not found.");
            }

            forumGroup.UpdateDetails(command.Name, command.PermissionSetId);

            _dbContext.Events.Add(new Event(nameof(ForumGroup), EventType.Updated, forumGroup.Id, command.MemberId, new
            {
                forumGroup.Name,
                forumGroup.PermissionSetId
            }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.ForumGroups(forumGroup.SiteId));
        }

        public async Task DeleteAsync(DeleteForumGroup command)
        {
            var forumGroup = await _dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == command.Id);

            if (forumGroup == null)
            {
                throw new DataException($"Forum Group with Id {command.Id} not found.");
            }

            forumGroup.Delete();
            _dbContext.Events.Add(new Event(nameof(ForumGroup), EventType.Deleted, forumGroup.Id, command.MemberId));

            var forums = await _dbContext.Forums
                .Where(x => x.ForumGroupId == forumGroup.Id)
                .ToListAsync();

            foreach (var forum in forums)
            {
                forum.Delete();
                _dbContext.Events.Add(new Event(nameof(Forum), EventType.Deleted, forum.Id, command.MemberId));
            }

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.ForumGroups(forumGroup.SiteId));
        }
    }
}
