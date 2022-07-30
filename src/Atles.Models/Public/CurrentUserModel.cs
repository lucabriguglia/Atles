namespace Atles.Models.Public
{
    public class CurrentUserModel
    {
        public Guid Id { get; set; }
        public string IdentityUserId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string GravatarHash { get; set; }
        public bool IsSuspended { get; set; }
    }
}