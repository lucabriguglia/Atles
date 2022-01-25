using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.Forums.Events
{
    /// <summary>
    /// Event published when a forum is moved
    /// </summary>
    public class ForumMoved : EventBase
    {
        /// <summary>
        /// The new sort order of the forum
        /// </summary>
        public int SortOrder { get; set; }
    }
}