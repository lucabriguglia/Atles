using System;

namespace Atlas.Domain.Categories.Events
{
    public class CategoryUpdated : EventBase
    {
        public string Name { get; set; }
        public Guid? PermissionSetId { get; set; }
    }
}
