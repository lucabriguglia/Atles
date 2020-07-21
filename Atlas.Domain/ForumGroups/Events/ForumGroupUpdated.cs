using System;

namespace Atlas.Domain.ForumGroups.Events
{
    public class ForumGroupUpdated : EventBase
    {
        public string Name { get; set; }
        public Guid? PermissionSetId { get; set; }
    }
}
