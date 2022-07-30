using Atles.Core.Commands;

namespace Atles.Commands.Categories
{
    /// <summary>
    /// Request that updates a forum category.
    /// </summary>
    public class UpdateCategory : CommandBase
    {
        public Guid CategoryId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The new name of the forum category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique identifier of the new permission set for the forum category.
        /// </summary>
        public Guid PermissionSetId { get; set; }
    }
}
