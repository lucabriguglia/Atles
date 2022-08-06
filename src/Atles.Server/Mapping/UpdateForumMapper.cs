using System;
using Atles.Commands.Forums;
using Atles.Models.Admin.Forums;

namespace Atles.Server.Mapping;

public class UpdateForumMapper : IMapper<ForumFormModelBase.ForumModel, UpdateForum>
{
    public UpdateForum Map(ForumFormModelBase.ForumModel model, Guid userId)
    {
        return new UpdateForum
        {
            ForumId = model.Id,
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
