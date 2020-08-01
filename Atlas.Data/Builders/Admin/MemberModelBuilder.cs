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
                    Email = member.Email,
                    TotalTopics = member.TopicsCount,
                    TotalReplies = member.RepliesCount
                });
            }

            return result;
        }

        public async Task<CreatePageModel> BuildCreatePageModelAsync()
        {
            await Task.CompletedTask;

            var result = new CreatePageModel();

            return result;
        }

        public async Task<EditPageModel> BuildEditPageModelAsync(Guid id)
        {
            var result = new EditPageModel();

            var member = await _dbContext.Members
                .FirstOrDefaultAsync(x =>
                    x.Id == id && 
                    x.Status != StatusType.Deleted);

            if (member == null)
            {
                return null;
            }

            result.Member = new EditPageModel.MemberModel
            {
                Id = member.Id,
                DisplayName = member.DisplayName
            };

            return result;
        }
    }
}
