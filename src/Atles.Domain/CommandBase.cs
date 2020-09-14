using System;

namespace Atles.Domain
{
    /// <summary>
    /// CommandBase
    /// </summary>
    public abstract class CommandBase
    {
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
