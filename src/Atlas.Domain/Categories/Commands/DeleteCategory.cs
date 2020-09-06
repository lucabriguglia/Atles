using System;

namespace Atlas.Domain.Categories.Commands
{
    /// <summary>
    /// Request that deletes a Forum Category.
    /// </summary>
    public class DeleteCategory : CommandBase
    {
        /// <summary>
        /// The unique identifier of the Forum Category to delete.
        /// </summary>
        public Guid Id { get; set; }
    }
}
