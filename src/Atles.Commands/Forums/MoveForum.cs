using Atles.Core.Commands;
using Atles.Domain;

namespace Atles.Commands.Forums
{
    /// <summary>
    /// Request that changes the sort order of a forum.
    /// </summary>
    public class MoveForum : CommandBase
    {
        public Guid ForumId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The direction.
        /// Can be either up or down.
        /// </summary>
        public DirectionType Direction { get; set; }
    }
}
