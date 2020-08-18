using System;
using System.Collections.Generic;

namespace Atlify.Domain.PermissionSets.Commands
{
    public class CreatePermissionSet : CommandBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public ICollection<PermissionCommand> Permissions { get; set; } = new List<PermissionCommand>();
    }
}
