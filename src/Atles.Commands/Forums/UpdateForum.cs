using Atles.Core.Commands;

namespace Atles.Commands.Forums
{
    /// <summary>
    /// Request that updates the forum details.
    /// </summary>
    public class UpdateForum : CommandBase
    {
        public Guid ForumId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The new forum category for the forum.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// The new name of the forum.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The new slug of the forum.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The new description of the forum.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The unique identifier of the new permission set for the forum.
        /// </summary>
        public Guid? PermissionSetId { get; set; }
    }
}
