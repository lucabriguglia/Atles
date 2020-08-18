using System;

namespace Atlify.Domain.PermissionSets.Commands
{
    public class DeletePermissionSet : CommandBase
    {
        public Guid Id { get; set; }
    }
}