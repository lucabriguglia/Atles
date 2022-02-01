using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request to suspend a user.
    /// </summary>
    [DocRequest(typeof(User))]
    public class SuspendUser : CommandBase
    {
    }
}