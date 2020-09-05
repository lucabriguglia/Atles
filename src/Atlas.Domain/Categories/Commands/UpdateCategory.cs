using System;
using Docs.Attributes;

namespace Atlas.Domain.Categories.Commands
{
    /// <summary>
    /// Request that updates a Forum Category.
    /// </summary>
    [DocRequest(typeof(Category))]
    public class UpdateCategory : CommandBase
    {
        /// <summary>
        /// The unique identifier of the Forum Category to update.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The new Name of the Forum Category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique identifier of the new Permission Set for the Forum Category.
        /// </summary>
        public Guid PermissionSetId { get; set; }
    }
}
