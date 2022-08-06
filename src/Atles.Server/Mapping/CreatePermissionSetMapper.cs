using Atles.Commands.PermissionSets;
using Atles.Models.Admin.PermissionSets;

namespace Atles.Server.Mapping;

public class CreatePermissionSetMapper : IMapper<PermissionSetFormModel.PermissionSetModel, CreatePermissionSet>
{
    public CreatePermissionSet Map(PermissionSetFormModel.PermissionSetModel model, Guid userId)
    {
        return new CreatePermissionSet
        {
            Name = model.Name,
            Permissions = model.Permissions.ToPermissionCommands(),
            SiteId = model.SiteId,
            UserId = userId
        };
    }
}
