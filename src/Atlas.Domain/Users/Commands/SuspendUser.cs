using System;
using Docs.Attributes;

namespace Atlas.Domain.Users.Commands
{
    /// <summary>
    /// Request to suspend a user.
    /// </summary>
    [DocRequest(typeof(User))]
    public class SuspendUser : CommandBase
    {
        /// <summary>
        /// The unique identifier of the user to be suspended.
        /// </summary>
        public Guid Id { get; set; }
    }
}