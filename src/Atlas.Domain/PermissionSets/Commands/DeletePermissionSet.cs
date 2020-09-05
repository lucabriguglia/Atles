using System;
using Docs.Attributes;

namespace Atlas.Domain.PermissionSets.Commands
{
    [DocRequest(typeof(PermissionSet))]
    public class DeletePermissionSet : CommandBase
    {
        public Guid Id { get; set; }
    }
}