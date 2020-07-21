using System;

namespace Atlas.Domain.Forums.Events
{
    public class ForumCreated : EventBase
    {
        public string Name { get; set; }
        public Guid? PermissionSetId { get; set; }
    }
}
