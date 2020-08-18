using System;

namespace Atlify.Models.Public.Members
{
    public class SettingsPageModel
    {
        public MemberModel Member { get; set; } = new MemberModel();

        public class MemberModel
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public string DisplayName { get; set; }
            public string GravatarHash { get; set; }
            public bool IsSuspended { get; set; }
        }
    }
}
