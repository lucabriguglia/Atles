using Atles.Data;
using Atles.Domain.Categories;
using Atles.Domain.Forums;
using Atles.Infrastructure.Queries;
using Atles.Models.Admin.Categories;
using Atles.Reporting.Admin.Categories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Admin.Categories
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
