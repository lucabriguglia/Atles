using System;

namespace ForumApp.Domain.Forums
{
    public class Forum
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid PermissionId { get; set; }
    }
}
