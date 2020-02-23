using System;

namespace ForumApp.Domain.Sites
{
    public class Site
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid PermissionId { get; set; }
    }
}
