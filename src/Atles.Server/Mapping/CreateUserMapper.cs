using Atles.Commands.Users;
using Atles.Models.Admin.Users;

namespace Atles.Server.Mapping;

public class CreateUserMapper : IMapper<CreatePageModel.UserModel, CreateUser>
{
    public CreateUser Map(CreatePageModel.UserModel model, Guid userId)
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
