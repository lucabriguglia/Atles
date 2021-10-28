using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Models.Forums.Commands
{
    /// <summary>
    /// Request that deletes a forum.
    /// </summary>
    [DocRequest(typeof(Forum))]
    public class DeleteForum : CommandBase
    {
    }
}
