using Atles.Commands.Categories;
using Atles.Models.Admin.Categories;

namespace Atles.Server.Mapping;

public class UpdateCategoryMapper : IMapper<CategoryFormModel.CategoryModel, UpdateCategory>
{
    public UpdateCategory Map(CategoryFormModel.CategoryModel model, Guid userId)
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