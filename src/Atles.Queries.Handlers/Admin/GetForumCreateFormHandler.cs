using System;
using System.Linq;
using System.Threading.Tasks;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Admin.Forums;
using Atles.Queries.Admin;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Admin
{
    public class GetForumCreateFormHandler : IQueryHandler<GetForumCreateForm, CreateForumFormModel>
    {
        private readonly AtlesDbContext _dbContext;

        public GetForumCreateFormHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<CreateForumFormModel>> Handle(GetForumCreateForm query)
        {
            var result = new CreateForumFormModel();

            var categories = await _dbContext.Categories
                .Where(x => x.SiteId == query.SiteId && x.Status != CategoryStatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            if (!categories.Any())
            {
                throw new ApplicationException("No Categories found.");
            }

            foreach (var category in categories)
            {
                result.Categories.Add(new CreateForumFormModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            var selectedCategoryId = query.CategoryId ?? categories.FirstOrDefault()?.Id ?? Guid.Empty;

            result.Forum = new CreateForumFormModel.ForumModel
            {
                CategoryId = selectedCategoryId
            };

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == query.SiteId && x.Status == PermissionSetStatusType.Published)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new CreateForumFormModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name
                });
            }

            return result;
        }
    }
}
