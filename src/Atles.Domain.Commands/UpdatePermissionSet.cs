using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request that updates a permission set.
    /// </summary>
    [DocRequest(typeof(PermissionSet))]
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