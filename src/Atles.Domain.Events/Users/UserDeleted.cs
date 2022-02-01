using Atles.Core.Events;

namespace Atles.Domain.Events.Users
{
    /// <summary>
    /// Event published when a user is deleted.
    /// </summary>
    public class UserDeleted : EventBase
    {
        public string IdentityUserId { get; set; }
    }
}
