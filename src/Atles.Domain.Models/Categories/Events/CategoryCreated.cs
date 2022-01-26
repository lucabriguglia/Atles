using System;
using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.Categories.Events
{
    /// <summary>
    /// Event published when a new category is created
    /// </summary>
    public class CategoryCreated : EventBase
    {
        /// <summary>
        /// The name of the new category
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The identifier of the permission set of the new category
        /// </summary>
        public Guid PermissionSetId { get; set; }

        /// <summary>
        /// The sort order of the new category
        /// </summary>
        public int SortOrder { get; set; }
    }
}
