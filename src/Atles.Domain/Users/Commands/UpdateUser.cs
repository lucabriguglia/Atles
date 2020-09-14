using System;
using System.Collections.Generic;
using Docs.Attributes;

namespace Atles.Domain.Users.Commands
{
    /// <summary>
    /// Request to update a user.
    /// </summary>
    [DocRequest(typeof(User))]
    public class UpdateUser : CommandBase
    {
        /// <summary>
        /// The unique identifier of the user to update.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The new display name of the user.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The list of roles to be assigned to the user.
        /// This value is populated only an administrator updates a user.
        /// </summary>
        public IList<string> Roles { get; set; } = null;
    }
}
