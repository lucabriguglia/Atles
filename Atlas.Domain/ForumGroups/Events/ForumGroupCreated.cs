using System;

namespace Atlas.Domain.ForumGroups.Events
{
    public class ForumGroupCreated : EventBase
    {
        public string Name { get; set; }
        public Guid? PermissionSetId { get; set; }
    }
}
