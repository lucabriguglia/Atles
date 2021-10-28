using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Models.Users.Commands
{
    /// <summary>
    /// Request to suspend a user.
    /// </summary>
    [DocRequest(typeof(User))]
    public class SuspendUser : CommandBase
    {
    }
}