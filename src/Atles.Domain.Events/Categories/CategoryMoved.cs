using Atles.Core.Events;

namespace Atles.Domain.Events.Categories
{
    /// <summary>
    /// Event published when a category is reordered
    /// </summary>
    public class CategoryMoved : EventBase
    {
        /// <summary>
        /// The new sort order of the category
        /// </summary>
        public int SortOrder { get; set; }
    }
}