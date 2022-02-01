using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request that deletes a forum.
    /// </summary>
    [DocRequest(typeof(Forum))]
    public class DeleteForum : CommandBase
    {
    }
}
