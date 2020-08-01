using System;

namespace Atlas.Domain.Forums.Events
{
    public class ForumCreated : EventBase
    {
        public Guid ForumGorupId { get; set; }
        public string Name { get; set; }
        public Guid? PermissionSetId { get; set; }
        public int SortOrder { get; set; }
    }
}
