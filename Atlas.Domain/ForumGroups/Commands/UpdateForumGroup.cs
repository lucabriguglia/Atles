using System;

namespace Atlas.Domain.ForumGroups.Commands
{
    public class UpdateForumGroup : CommandBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? PermissionSetId { get; set; }
    }
}
