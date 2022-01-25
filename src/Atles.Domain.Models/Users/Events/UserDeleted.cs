using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.Users.Events
{
    /// <summary>
    /// Event published when a user is deleted.
    /// </summary>
    public class UserDeleted : EventBase
    {
        public string IdentityUserId { get; set; }
    }
}
