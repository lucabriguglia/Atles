using System;
using Atles.Commands.Forums;
using Atles.Models.Admin.Forums;

namespace Atles.Server.Mapping;

public class CreateForumMapper : IMapper<CreateForumFormModel.ForumModel, CreateForum>
{
    public CreateForum Map(CreateForumFormModel.ForumModel model, Guid userId)
    {
        return new CreateForum
        {
            Name = model.Name,
            Slug = model.Slug,
            Description = model.Description,
            PermissionSetId = model.PermissionSetId,
            SiteId = model.SiteId,
            UserId = userId
        };
    }
}
