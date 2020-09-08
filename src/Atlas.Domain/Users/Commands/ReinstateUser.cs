using System;
using Docs.Attributes;

namespace Atlas.Domain.Users.Commands
{
    /// <summary>
    /// Request to re-active a suspended user.
    /// </summary>
    [DocRequest(typeof(User))]
    public class ReinstateUser : CommandBase
    {
        /// <summary>
        /// The unique identifier of the user to re-activate.
        /// </summary>
        public Guid Id { get; set; }
    }
}