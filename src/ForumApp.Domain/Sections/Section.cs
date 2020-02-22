using System;

namespace ForumApp.Domain.Sections
{
    public class Section
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public int Topics { get; set; }
        public int Replies { get; set; }
        public Guid? PermissionId { get; set; }
    }
}
