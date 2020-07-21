using Atlas.Domain;
using Atlas.Shared;
using Atlas.Shared.Models.Admin.ForumGroups;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Data.Builders.Admin
{
    public class ForumGroupModelBuilder : IForumGroupModelBuilder
    {
        private readonly AtlasDbContext _dbContext;

        public ForumGroupModelBuilder(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IndexModel> BuildIndexModelAsync(Guid siteId)
        {
            var result = new IndexModel();

            var forumGroups = await _dbContext.ForumGroups
                .Include(x => x.PermissionSet)
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var forumGroup in forumGroups)
            {
                result.ForumGroups.Add(new IndexModel.ForumGroupModel
                {
                    Id = forumGroup.Id,
                    Name = forumGroup.Name,
                    SortOrder = forumGroup.SortOrder,
                    TotalTopics = forumGroup.TopicsCount,
                    TotalReplies = forumGroup.RepliesCount
                });
            }

            return result;
        }

        public async Task<FormModel> BuildFormModelAsync(Guid siteId, Guid? id = null)
        {
            var result = new FormModel();

            if (id != null)
            {
                var forumGroup = await _dbContext.ForumGroups
                    .FirstOrDefaultAsync(x => 
                        x.SiteId == siteId && 
                        x.Id == id && 
                        x.Status != StatusType.Deleted);

                if (forumGroup == null)
                {
                    return null;
                }

                result.ForumGroup = new FormModel.ForumGroupModel
                {
                    Id = forumGroup.Id,
                    Name = forumGroup.Name,
                    PermissionSetId = forumGroup.PermissionSetId ?? Guid.Empty
                };
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
