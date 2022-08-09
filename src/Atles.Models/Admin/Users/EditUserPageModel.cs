using Atles.Domain;

namespace Atles.Models.Admin.Users;

public class EditUserPageModel
{
    public UserModel User { get; set; } = new();
    public InfoModel Info { get; set; } = new();
    
    public class UserModel : SiteFormModel
    {
        public Guid Id { get; set; }
        public string IdentityUserId { get; set; }
        public string DisplayName { get; set; }
        public IList<RoleModel> Roles { get; set; } = new List<RoleModel>();
    }

    public class InfoModel
    {
        public UserStatusType Status { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }

    public class RoleModel
    {
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}
