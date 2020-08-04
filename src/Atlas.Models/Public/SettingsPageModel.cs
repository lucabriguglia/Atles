using System;

namespace Atlas.Models.Public
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
        }
    }
}
