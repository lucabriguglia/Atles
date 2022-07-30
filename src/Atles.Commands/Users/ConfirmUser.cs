using Atles.Core.Commands;
using Atles.Domain;
using Docs.Attributes;

namespace Atles.Commands.Users
{
    /// <summary>
    /// Request to set the status of a user to active after the email has been confirmed.
    /// </summary>
    [DocRequest(typeof(User))]
    public class ConfirmUser : CommandBase
    {
        public string IdentityUserId { get; set; }
    }
}