using Atles.Core.Commands;

namespace Atles.Commands.PermissionSets
{
    /// <summary>
    /// Request that updates a permission set.
    /// </summary>
    public class UpdatePermissionSet : CommandBase
    {
        public Guid PermissionSetId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The new name for the permission set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The new permissions for the permission set.
        /// </summary>
        public ICollection<PermissionCommand> Permissions { get; set; } = new List<PermissionCommand>();
    }
}