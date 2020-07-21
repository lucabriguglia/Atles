using System;

namespace Atlas.Domain.PermissionSets.Events
{
    public class PermissionSetCreated : EventBase
    {
        public string Name { get; set; }
    }
}
