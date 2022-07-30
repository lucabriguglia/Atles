using Atles.Core.Events;

namespace Atles.Events.Users
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
