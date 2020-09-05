using System;
using Docs.Attributes;

namespace Atlas.Domain.Categories.Commands
{
    /// <summary>
    /// Request that deletes a Forum Category.
    /// </summary>
    [DocRequest(typeof(Category))]
    public class DeleteCategory : CommandBase
    {
        /// <summary>
        /// The unique identifier of the Forum Category to delete.
        /// </summary>
        public Guid Id { get; set; }
    }
}
