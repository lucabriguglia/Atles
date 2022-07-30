using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Commands.Users
{
    /// <summary>
    /// Request to suspend a user.
    /// </summary>
    [DocRequest(typeof(User))]
    public class SuspendUser : CommandBase
    {
        public Guid SuspendUserId { get; set; } = Guid.NewGuid();
    }
}