using Atles.Domain.Models;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request that changes the sort order of a forum category.
    /// </summary>
    [DocRequest(typeof(Category))]
    public class MoveCategory : CommandBase
    {
        /// <summary>
        /// The direction.
        /// Can be either up or down.
        /// </summary>
        public Direction Direction { get; set; }
    }
}
