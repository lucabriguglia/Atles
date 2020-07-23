using Atlas.Domain;
using Atlas.Shared.Forums;
using Atlas.Shared.Models.Admin.Forums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Data.Builders.Admin
{
    public class ForumModelBuilder : IForumModelBuilder
    {
        private readonly AtlasDbContext _dbContext;

        public ForumModelBuilder(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IndexModel> BuildIndexModelAsync(Guid siteId, Guid? forumGroupId = null)
        {
            var forumGroups = await _dbContext.ForumGroups
                .Include(x => x.PermissionSet)
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            if (!forumGroups.Any())
            {
                throw new ApplicationException("No Forum Groups found");
            }

            var currentForumGroup = forumGroupId == null
                ? forumGroups.FirstOrDefault()
                : forumGroups.FirstOrDefault(x => x.Id == forumGroupId);

            if (currentForumGroup == null)
            {
                throw new ApplicationException("Forum Group not found");
            }

            var forums = await _dbContext.Forums
                .Include(x => x.PermissionSet)
                .Where(x => x.ForumGroupId == currentForumGroup.Id && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            var result = new IndexModel();

            foreach (var forumGroup in forumGroups)
            {
                result.ForumGroups.Add(new IndexModel.ForumGroupModel
                {
                    Id = forumGroup.Id,
                    Name = forumGroup.Name
                });
            }

            foreach (var forum in forums)
            {
                var permissionSetName = !forum.HasPermissionSet()
                    ? !currentForumGroup.HasPermissionSet()
                        ? "Default (from Site)"
                        : $"{currentForumGroup.PermissionSetName()} (from Group)"
                    : forum.PermissionSetName();

                result.Forums.Add(new IndexModel.ForumModel
                {
                    Id = forum.Id,
                    Name = forum.Name,
                    SortOrder = forum.SortOrder,
                    TotalTopics = forum.TopicsCount,
                    TotalReplies = forum.RepliesCount,
                    PermissionSetName = permissionSetName
                });
            }

            return result;
        }

        public async Task<FormModel> BuildCreateFormModelAsync(Guid siteId, Guid? forumGroupId = null)
        {
            var result = new FormModel();

            var forumGroups = await _dbContext.ForumGroups
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var forumGroup in forumGroups)
            {
                result.ForumGroups.Add(new FormModel.ForumGroupModel
                {
                    Id = forumGroup.Id,
                    Name = forumGroup.Name
                });
            }

            var selectedForumGroupId = forumGroupId == null 
                ? forumGroups.FirstOrDefault().Id 
                : forumGroupId.Value;

            result.Forum = new FormModel.ForumModel
            {
                ForumGroupId = selectedForumGroupId
            };

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new FormModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name
                });
            }

            return result;
        }

        public async Task<FormModel> BuildEditFormModelAsync(Guid siteId, Guid id)
        {
            var forum = await _dbContext.Forums
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.Status != StatusType.Deleted);

            if (forum == null)
            {
                return null;
            }

            var result = new FormModel
            {
                Forum = new FormModel.ForumModel
                {
                    Id = forum.Id,
                    ForumGroupId = forum.ForumGroupId,
                    Name = forum.Name,
                    PermissionSetId = forum.PermissionSetId ?? Guid.Empty
                }
            };

            var forumGroups = await _dbContext.ForumGroups
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var forumGroup in forumGroups)
            {
                result.ForumGroups.Add(new FormModel.ForumGroupModel
                {
                    Id = forumGroup.Id,
                    Name = forumGroup.Name
                });
            }

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new FormModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name
                });
            }

            return result;
        }
    }
}
