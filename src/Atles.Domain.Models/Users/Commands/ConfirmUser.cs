using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Models.Users.Commands
{
    /// <summary>
    /// Request to set the status of a user to active after the email has been confirmed.
    /// </summary>
    [DocRequest(typeof(User))]
    public class ConfirmUser : CommandBase
    {
    }
}