using Atles.Core.Events;
using Atles.Domain;

namespace Atles.Events.PermissionSets
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
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}