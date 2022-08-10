using Atles.Commands.Users;
using Atles.Models.Admin.Users;

namespace Atles.Server.Mapping;

public class UpdateUserMapper : IMapper<EditUserPageModel.UserModel, UpdateUser>
{
    public UpdateUser Map(EditUserPageModel.UserModel model, Guid userId)
    {
        return new UpdateUser
        {
            Id = model.Id,
            IdentityUserId = model.IdentityUserId,
            DisplayName = model.DisplayName,
            Roles = model.Roles.Select(role => new Role(role.Name, role.Selected)).ToList(),
            SiteId = model.SiteId,
            UserId = userId
        };
    }
}
