using System;

namespace Atlas.Domain.PermissionSets.Commands
{
    public class CreatePermissionSet : CommandBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
}
