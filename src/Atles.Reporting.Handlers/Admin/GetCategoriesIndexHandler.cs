using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Models.Categories;
using Atles.Domain.Models.Forums;
using Atles.Reporting.Models.Admin.Categories;
using Atles.Reporting.Models.Admin.Categories.Queries;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Queries;

namespace Atles.Reporting.Handlers.Admin
{
    public class GetCategoriesIndexHandler : IQueryHandler<GetCategoriesIndex, IndexPageModel>
    {
        private readonly AtlesDbContext _dbContext;

        public GetCategoriesIndexHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IndexPageModel> Handle(GetCategoriesIndex query)
        {
            var result = new IndexPageModel();

            var categories = await _dbContext.Categories
                .Include(x => x.PermissionSet)
                .Where(x => x.SiteId == query.SiteId && x.Status != CategoryStatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var category in categories)
            {
                var forumsCount = await _dbContext.Forums
                    .Where(x =>
                        x.CategoryId == category.Id &&
                        x.Status != ForumStatusType.Deleted)
                    .CountAsync();

                result.Categories.Add(new IndexPageModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    SortOrder = category.SortOrder,
                    TotalForums = forumsCount,
                    TotalTopics = category.TopicsCount,
                    TotalReplies = category.RepliesCount,
                    PermissionSetName = category.PermissionSetName()
                });
            }

            return result;
        }
    }
}
