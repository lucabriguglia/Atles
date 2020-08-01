using System;

namespace Atlas.Domain.Categories.Events
{
    public class CategoryCreated : EventBase
    {
        public string Name { get; set; }
        public Guid? PermissionSetId { get; set; }
        public int SortOrder { get; set; }
    }
}
