using System;

namespace Atles.Domain.Categories.Commands
{
    /// <summary>
    /// Request that creates a new Forum Category.
    /// </summary>
    public class CreateCategory : CommandBase
    {
        /// <summary>
        /// The unique identifier for the new Forum Category.
        /// If not specified, a random one is assigned.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The name for the Forum Category.
        /// This value is mandatory.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique identifier of the Permission Set used os default for all Forums within the Forum Category.
        /// This value is mandatory.
        /// </summary>
        public Guid PermissionSetId { get; set; }
    }
}
