using System;

namespace Atlas.Domain.PermissionSets.Commands
{
    public class DeletePermissionSet : CommandBase
    {
        public Guid Id { get; set; }
    }
}