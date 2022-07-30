using Atles.Core.Events;

namespace Atles.Events.Categories
{
    /// <summary>
    /// Event published when a category is updated
    /// </summary>
    public class CategoryUpdated : EventBase
    {
        /// <summary>
        /// The new name of the category
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The new identifier of the permission set of the category
        /// </summary>
        public Guid PermissionSetId { get; set; }
    }
}