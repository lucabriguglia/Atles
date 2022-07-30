using Atles.Core.Commands;

namespace Atles.Commands.PermissionSets
{
    /// <summary>
    /// Request that deletes a permission set.
    /// </summary>
    public class DeletePermissionSet : CommandBase
    {
        public Guid PermissionSetId { get; set; } = Guid.NewGuid();
    }
}