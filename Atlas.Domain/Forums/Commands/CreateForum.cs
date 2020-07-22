using System;

namespace Atlas.Domain.Forums.Commands
{
    public class CreateForum : CommandBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ForumGroupId { get; set; }
        public string Name { get; set; }
        public Guid? PermissionSetId { get; set; }
    }
}
