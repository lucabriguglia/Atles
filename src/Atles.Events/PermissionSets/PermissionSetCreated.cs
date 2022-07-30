using Atles.Core.Events;
using Atles.Domain;

namespace Atles.Events.PermissionSets
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
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
