using Atles.Infrastructure.Commands;
using System;

namespace Atles.Domain.Categories.Commands
{
    /// <summary>
    /// Request that changes the sort order of a forum category.
    /// </summary>
    public class MoveCategory : CommandBase
    {
        /// <summary>
        /// The direction.
        /// Can be either up or down.
        /// </summary>
        public Direction Direction { get; set; }
    }
}
