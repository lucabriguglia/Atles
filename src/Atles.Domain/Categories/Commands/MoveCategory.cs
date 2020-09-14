using System;

namespace Atlas.Domain.Categories.Commands
{
    /// <summary>
    /// Request that changes the sort order of a forum category.
    /// </summary>
    public class MoveCategory : CommandBase
    {
        /// <summary>
        /// The unique identifier of the forum category to move.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The direction.
        /// Can be either up or down.
        /// </summary>
        public Direction Direction { get; set; }
    }
}
