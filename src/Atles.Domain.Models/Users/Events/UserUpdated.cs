using System.Collections.Generic;
using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.Users.Events
{
    /// <summary>
    /// Event published when a user is updated.
    /// </summary>
    public class UserUpdated : EventBase
    {
        /// <summary>
        /// The new display name of the user.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The list of roles assigned to the user.
        /// </summary>
        public string Roles { get; set; }
    }
}
