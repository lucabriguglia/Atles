using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.Forums.Events
{
    /// <summary>
    /// Event published when a forum is reordered
    /// </summary>
    public class ForumReordered : EventBase
    {
        /// <summary>
        /// New sort order
        /// </summary>
        public int SortOrder { get; set; }
    }
}