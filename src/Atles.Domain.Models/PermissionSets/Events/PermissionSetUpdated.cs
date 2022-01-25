using System.Collections.Generic;
using Atles.Domain.Models.PermissionSets.Commands;
using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.PermissionSets.Events
{
    /// <summary>
    /// Event published when a permission set is updated.
    /// </summary>
    public class PermissionSetUpdated : EventBase
    {
        /// <summary>
        /// The new name for the permission set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The new permissions for the permission set.
        /// </summary>
        public ICollection<PermissionCommand> Permissions { get; set; } = new List<PermissionCommand>();
    }
}