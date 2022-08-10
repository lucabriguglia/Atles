namespace Atles.Models.Admin.Users;

public class CreateUserPageModel
{
    public UserModel User { get; set; } = new();

    public class UserModel : SiteFormModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}