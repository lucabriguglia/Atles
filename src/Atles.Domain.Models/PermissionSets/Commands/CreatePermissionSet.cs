using Atles.Infrastructure.Commands;
using System;
using System.Collections.Generic;
using Docs.Attributes;

namespace Atles.Domain.PermissionSets.Commands
{
    /// <summary>
    /// Request that creates a new permission set.
    /// </summary>
    [DocRequest(typeof(PermissionSet))]
    public class CreatePermissionSet : CommandBase
    {
        /// <summary>
        /// The name of the new permission set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The permissions of the new permission set.
        /// </summary>
        public ICollection<PermissionCommand> Permissions { get; set; } = new List<PermissionCommand>();
    }
}
