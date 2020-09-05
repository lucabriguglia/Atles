using System;
using Docs.Attributes;

namespace Atlas.Domain.Categories.Commands
{
    /// <summary>
    /// Request that changes the Sort Order of a Forum Category.
    /// </summary>
    [DocRequest(typeof(Category))]
    public class MoveCategory : CommandBase
    {
        /// <summary>
        /// The unique identifier of the Forum Category to move.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The direction. Can be either Up or Down.
        /// </summary>
        public Direction Direction { get; set; }
    }
}
