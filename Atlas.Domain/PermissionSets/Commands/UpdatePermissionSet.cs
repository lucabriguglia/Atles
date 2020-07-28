using System;

namespace Atlas.Domain.PermissionSets.Commands
{
    public class UpdatePermissionSet : CommandBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}