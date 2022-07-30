namespace Atles.Models.Public
{
    public class SettingsPageModel
    {
        public UserModel User { get; set; } = new UserModel();

        public class UserModel
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public string DisplayName { get; set; }
            public string GravatarHash { get; set; }
            public bool IsSuspended { get; set; }
        }
    }
}
