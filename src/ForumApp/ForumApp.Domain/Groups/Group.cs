using System;

namespace ForumApp.Domain.Groups
{
    public class Group
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public string Name { get; set; }
        public int Topics { get; set; }
        public int Replies { get; set; }
        public Guid? PermissionId { get; set; }
    }    
}
