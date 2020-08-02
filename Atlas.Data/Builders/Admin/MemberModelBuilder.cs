using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Models.Admin.Members;
using Atlas.Models;

namespace Atlas.Data.Builders.Admin
{
    public class MemberModelBuilder : IMemberModelBuilder
    {
        private readonly AtlasDbContext _dbContext;

        public MemberModelBuilder(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IndexPageModel> BuildIndexPageModelAsync(PaginationOptions options)
        {
            var result = new IndexPageModel();

            var members = await _dbContext.Members
                .Where(x => x.Status != StatusType.Deleted)
                .OrderBy(x => x.DisplayName)
                .Skip(options.Skip)
                .Take(options.PageSize)
                .ToListAsync();

            var items = members.Select(member => new IndexPageModel.MemberModel
            {
                Id = member.Id,
                DisplayName = member.DisplayName,
                Email = member.Email,
                TotalTopics = member.TopicsCount,
                TotalReplies = member.RepliesCount
            })
            .ToList();

            var totalRecords = await _dbContext.Members
                .Where(x => x.Status == StatusType.Active)
                .CountAsync();

            result.Members = new PaginatedData<IndexPageModel.MemberModel>(items, totalRecords, options.PageSize);

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
