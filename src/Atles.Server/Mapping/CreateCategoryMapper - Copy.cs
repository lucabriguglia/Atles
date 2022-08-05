using System;
using Atles.Commands.Categories;
using Atles.Models.Admin;

namespace Atles.Server.Mapping;

public class UpdateCategoryMapper : IMapper<CategoryFormModelBase.CategoryModel, UpdateCategory>
{
    public UpdateCategory Map(CategoryFormModelBase.CategoryModel model, Guid userId)
    {
        return new UpdateCategory
        {
            CategoryId = model.Id,
            Name = model.Name,
            PermissionSetId = model.PermissionSetId,
            SiteId = model.SiteId,
            UserId = userId
        };
    }
}