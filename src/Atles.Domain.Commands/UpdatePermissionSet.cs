using System.Collections.Generic;
using Atles.Domain.Models.PermissionSets;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request that updates a permission set.
    /// </summary>
    [DocRequest(typeof(PermissionSet))]
    public class UpdatePermissionSet : CommandBase
    {
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