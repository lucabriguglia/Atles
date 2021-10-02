using Atles.Infrastructure.Commands;
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
    }
}