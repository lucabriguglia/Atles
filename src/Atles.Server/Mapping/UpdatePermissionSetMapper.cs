using Atles.Commands.PermissionSets;
using Atles.Models.Admin.PermissionSets;

namespace Atles.Server.Mapping;

public class UpdatePermissionSetMapper : IMapper<PermissionSetFormModel.PermissionSetModel, UpdatePermissionSet>
{
    public UpdatePermissionSet Map(PermissionSetFormModel.PermissionSetModel model, Guid userId)
    {
        return new UpdatePermissionSet
        {
            PermissionSetId = model.Id,
            Name = model.Name,
            Permissions = model.Permissions.ToPermissionCommands(),
            SiteId = model.SiteId,
            UserId = userId
        };
    }
}
