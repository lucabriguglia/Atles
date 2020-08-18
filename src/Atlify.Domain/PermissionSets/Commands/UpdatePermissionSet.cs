using System;
using System.Collections.Generic;

namespace Atlify.Domain.PermissionSets.Commands
{
    public class UpdatePermissionSet : CommandBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<PermissionCommand> Permissions { get; set; } = new List<PermissionCommand>();
    }
}