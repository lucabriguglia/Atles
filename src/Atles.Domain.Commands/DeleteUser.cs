using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request to set the status of a user to deleted and to remove the identity user from the membership database.
    /// </summary>
    [DocRequest(typeof(User))]
    public class DeleteUser : CommandBase
    {
        public string IdentityUserId { get; set; }
    }
}
