using Atles.Core.Commands;

namespace Atles.Commands.PermissionSets
{
    /// <summary>
    /// Request that creates a new permission set.
    /// </summary>
    public class CreatePermissionSet : CommandBase
    {
        public Guid PermissionSetId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The name of the new permission set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The permissions of the new permission set.
        /// </summary>
        public ICollection<PermissionCommand> Permissions { get; set; } = new List<PermissionCommand>();
    }
}
