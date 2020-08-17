using System;

namespace Atlas.Models.Public
{
    public class CurrentMemberModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string GravatarHash { get; set; }
        public bool Suspended { get; set; }
    }
}