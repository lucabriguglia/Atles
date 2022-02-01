using Atles.Domain.Models;
using Atles.Infrastructure.Events;

namespace Atles.Domain.Events
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
