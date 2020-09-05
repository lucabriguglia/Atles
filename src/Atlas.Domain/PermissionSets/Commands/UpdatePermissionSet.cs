using System;
using System.Collections.Generic;
using Docs.Attributes;

namespace Atlas.Domain.PermissionSets.Commands
{
    [DocRequest(typeof(PermissionSet))]
    public class UpdatePermissionSet : CommandBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<PermissionCommand> Permissions { get; set; } = new List<PermissionCommand>();
    }
}