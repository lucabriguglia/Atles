using System;
using Docs.Attributes;

namespace Atles.Domain.Users.Commands
{
    /// <summary>
    /// Request to set the status of a user to deleted and to remove the identity user from the membership database.
    /// </summary>
    [DocRequest(typeof(User))]
    public class DeleteUser : CommandBase
    {
        /// <summary>
        /// The unique identifier of the user to delete.
        /// </summary>
        public Guid Id { get; set; }
    }
}
