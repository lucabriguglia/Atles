using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Models.Users.Commands
{
    /// <summary>
    /// Request to re-active a suspended user.
    /// </summary>
    [DocRequest(typeof(User))]
    public class ReinstateUser : CommandBase
    {
    }
}