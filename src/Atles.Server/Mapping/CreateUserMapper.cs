using Atles.Commands.Users;
using Atles.Models.Admin.Users;

namespace Atles.Server.Mapping;

public class CreateUserMapper : IMapper<CreateUserPageModel.UserModel, CreateUser>
{
    public CreateUser Map(CreateUserPageModel.UserModel model, Guid userId)
    {
        return new CreateUser
        {
            Email = model.Email,
            Password = model.Password,
            SiteId = model.SiteId,
            UserId = userId,
            Confirm = true
        };
    }
}
