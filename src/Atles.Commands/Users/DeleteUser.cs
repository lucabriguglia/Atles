using Atles.Core.Commands;
using Atles.Domain;
using Docs.Attributes;

namespace Atles.Commands.Users
{
    /// <summary>
    /// Request to set the status of a user to deleted and to remove the identity user from the membership database.
    /// </summary>
    [DocRequest(typeof(User))]
    public class DeleteUser : CommandBase
    {
        public Guid DeleteUserId { get; set; } = Guid.NewGuid();
        public string IdentityUserId { get; set; }
    }
}
