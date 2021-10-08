using OpenCqrs.Commands;
using System;

namespace Atles.Infrastructure.Commands
{
    /// <summary>
    /// CommandBase
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        /// <summary>
        /// The unique identifier of the Aggregate.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The unique identifier of the Site whose the request belongs to.
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// The unique identifier of the User who initiated the request.
        /// </summary>
        public Guid UserId { get; set; }
    }
}
