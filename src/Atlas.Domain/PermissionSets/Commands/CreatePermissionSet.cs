using System;
using System.Collections.Generic;
using Docs.Attributes;

namespace Atlas.Domain.PermissionSets.Commands
{
    [DocRequest(typeof(PermissionSet))]
    public class CreatePermissionSet : CommandBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public ICollection<PermissionCommand> Permissions { get; set; } = new List<PermissionCommand>();
    }
}
