using Atles.Domain.Models;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request that deletes a permission set.
    /// </summary>
    [DocRequest(typeof(PermissionSet))]
    public class DeletePermissionSet : CommandBase
    {
    }
}