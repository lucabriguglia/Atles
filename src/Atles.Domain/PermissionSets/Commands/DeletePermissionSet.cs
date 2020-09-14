using System;
using Docs.Attributes;

namespace Atles.Domain.PermissionSets.Commands
{
    /// <summary>
    /// Request that deletes a permission set.
    /// </summary>
    [DocRequest(typeof(PermissionSet))]
    public class DeletePermissionSet : CommandBase
    {
        /// <summary>
        /// The unique identifier of the permission set to delete.
        /// </summary>
        public Guid Id { get; set; }
    }
}