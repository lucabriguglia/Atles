using System;
using Atles.Commands.Categories;
using Atles.Models.Admin;

namespace Atles.Server.Mapping;

public class CreateCategoryMapper : IMapper<CategoryFormModel.CategoryModel, CreateCategory>
{
    public CreateCategory Map(CategoryFormModel.CategoryModel model, Guid userId)
    {
        return new CreateCategory
        {
            Name = model.Name,
            PermissionSetId = model.PermissionSetId,
            SiteId = model.SiteId,
            UserId = userId
        };
    }
}