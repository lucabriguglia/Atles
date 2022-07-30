using Atles.Core.Commands;
using Atles.Domain;
using Docs.Attributes;

namespace Atles.Commands.Users;

/// <summary>
/// Request to change the email of the user.
/// </summary>
[DocRequest(typeof(User))]
public class ChangeEmail : CommandBase
{
    public string IdentityUserId { get; set; }
    public string Email { get; set; }
}