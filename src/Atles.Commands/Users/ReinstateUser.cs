using Atles.Core.Commands;
using Atles.Domain;
using Docs.Attributes;

namespace Atles.Commands.Users
{
    /// <summary>
    /// Request to re-active a suspended user.
    /// </summary>
    [DocRequest(typeof(User))]
    public class ReinstateUser : CommandBase
    {
        public Guid ReinstateUserId { get; set; } = Guid.NewGuid();
    }
}