using Atles.Core.Events;
using Atles.Domain;

namespace Atles.Events.Users
{
    /// <summary>
    /// Event published when a user is created.
    /// </summary>
    public class UserCreated : EventBase
    {
        /// <summary>
        /// The unique identifier of the user in the membership database.
        /// </summary>
        public string IdentityUserId { get; set; }

        /// <summary>
        /// The email of the user in the membership database.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The status.
        /// </summary>
        public UserStatusType Status { get; set; }
    }
}
