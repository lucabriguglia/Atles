using System;
using Docs.Attributes;

namespace Atles.Domain.Forums.Commands
{
    /// <summary>
    /// Request that deletes a forum.
    /// </summary>
    [DocRequest(typeof(Forum))]
    public class DeleteForum : CommandBase
    {
        /// <summary>
        /// The unique identifier of the forum to delete.
        /// </summary>
        public Guid Id { get; set; }
    }
}
