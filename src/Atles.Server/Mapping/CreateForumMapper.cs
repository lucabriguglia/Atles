using Atles.Commands.Forums;
using Atles.Models.Admin.Forums;

namespace Atles.Server.Mapping;

public class CreateForumMapper : IMapper<ForumFormModel.ForumModel, CreateForum>
{
    public CreateForum Map(ForumFormModel.ForumModel model, Guid userId)
    {
        return new CreateForum
        {
            Name = model.Name,
            Slug = model.Slug,
            Description = model.Description,
            PermissionSetId = model.PermissionSetId != Guid.Empty ? model.PermissionSetId : null,
            CategoryId = model.CategoryId,
            SiteId = model.SiteId,
            UserId = userId
        };
    }
}
