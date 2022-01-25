using System.Collections.Generic;
using Atles.Domain.Models.PermissionSets.Commands;
using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.PermissionSets.Events
{
    /// <summary>
    /// Event published when a new permission set is created.
    /// </summary>
    public class PermissionSetCreated : EventBase
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
