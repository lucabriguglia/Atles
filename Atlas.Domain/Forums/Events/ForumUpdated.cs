using System;

namespace Atlas.Domain.Forums.Events
{
    public class ForumUpdated : EventBase
    {
        public Guid ForumGroupId { get; set; }
        public string Name { get; set; }
        public Guid? PermissionSetId { get; set; }
    }
}
