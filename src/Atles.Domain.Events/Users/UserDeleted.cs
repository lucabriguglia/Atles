using Atles.Core.Events;

namespace Atles.Events.Users
{
    /// <summary>
    /// Event published when a user is deleted.
    /// </summary>
    public class UserDeleted : EventBase
    {
        public int RemovedSubscriptions { get; set; }
    }
}
