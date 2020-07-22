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

        public async Task<IndexModel> BuildIndexModelAsync(Guid forumGroupId)
        {
            var result = new IndexModel();

            var forums = await _dbContext.Forums
                .Include(x => x.PermissionSet)
                .Where(x => x.ForumGroupId == forumGroupId && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var forum in forums)
            {
                result.Forums.Add(new IndexModel.ForumModel
                {
                    Id = forum.Id,
                    Name = forum.Name,
                    SortOrder = forum.SortOrder,
                    TotalTopics = forum.TopicsCount,
                    TotalReplies = forum.RepliesCount
                });
            }

            return result;
        }

        public async Task<FormModel> BuildCreateFormModelAsync(Guid siteId, Guid forumGroupId)
        {
            var result = new FormModel
            {
                Forum = new FormModel.ForumModel
                {
                    ForumGroupId = forumGroupId
                }
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
