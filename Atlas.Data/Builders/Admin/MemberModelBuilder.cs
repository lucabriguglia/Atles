using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Models.Admin.Members;

namespace Atlas.Data.Builders.Admin
{
    public class MemberModelBuilder : IMemberModelBuilder
    {
        private readonly AtlasDbContext _dbContext;

        public MemberModelBuilder(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IndexPageModel> BuildIndexPageModelAsync()
        {
            var result = new IndexPageModel();

            var members = await _dbContext.Members
                .Where(x => x.Status != StatusType.Deleted)
                .OrderBy(x => x.DisplayName)
                .ToListAsync();

            foreach (var member in members)
            {
                result.Members.Add(new IndexPageModel.MemberModel
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName,
                    TotalTopics = member.TopicsCount,
                    TotalReplies = member.RepliesCount
                });
            }

            return result;
        }

        public async Task<FormComponentModel> BuildFormModelAsync(Guid? id = null)
        {
            var result = new FormComponentModel();

            if (id != null)
            {
                var member = await _dbContext.Members
                    .FirstOrDefaultAsync(x =>
                        x.Id == id && 
                        x.Status != StatusType.Deleted);

                if (member == null)
                {
                    return null;
                }

                result.Member = new FormComponentModel.MemberModel
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName
                };
            }
            else
            {
                result.Member = new FormComponentModel.MemberModel
                {
                };
            }

            return result;
        }
    }
}
