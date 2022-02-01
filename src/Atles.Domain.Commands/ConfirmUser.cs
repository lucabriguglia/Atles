using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request to set the status of a user to active after the email has been confirmed.
    /// </summary>
    [DocRequest(typeof(User))]
    public class ConfirmUser : CommandBase
    {
    }
}