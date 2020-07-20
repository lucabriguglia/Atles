using System;

namespace Atlas.Domain.ForumGroups.Commands
{
    public class CreateForumGroup : CommandBase
    {
        public string Name { get; set; }
        public Guid? PermissionSetId { get; set; }
    }


}
