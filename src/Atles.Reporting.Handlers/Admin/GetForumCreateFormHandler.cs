using Atles.Data;
using Atles.Domain.Categories;
using Atles.Domain.PermissionSets;
using Atles.Models.Admin.Forums;
using Atles.Reporting.Admin.Forums;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Queries;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Admin.Forums
{
    public class GetForumCreateFormHandler : IQueryHandler<GetForumCreateForm, FormComponentModel>
    {
        private readonly AtlesDbContext _dbContext;

        public GetForumCreateFormHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FormComponentModel> Handle(GetForumCreateForm query)
        {
            var result = new FormComponentModel();

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
                result.Categories.Add(new FormComponentModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            var selectedCategoryId = query.CategoryId ?? categories.FirstOrDefault()?.Id ?? Guid.Empty;

            result.Forum = new FormComponentModel.ForumModel
            {
                CategoryId = selectedCategoryId
            };

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == query.SiteId && x.Status == PermissionSetStatusType.Published)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new FormComponentModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name
                });
            }

            return result;
        }
    }
}
