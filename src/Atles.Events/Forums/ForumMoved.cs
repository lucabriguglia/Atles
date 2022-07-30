using Atles.Core.Events;

namespace Atles.Events.Forums
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