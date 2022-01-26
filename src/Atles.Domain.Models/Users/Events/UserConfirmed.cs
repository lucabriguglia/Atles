using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.Users.Events
{
    /// <summary>
    /// Event published when a user has confirmed their email.
    /// </summary>
    public class UserConfirmed : EventBase
    {
    }
}