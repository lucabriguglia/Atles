using Atles.Core.Commands;
using Atles.Domain;
using Docs.Attributes;

namespace Atles.Commands.Users;

/// <summary>
/// Request to update a user.
/// </summary>
[DocRequest(typeof(User))]
public class UpdateUser : CommandBase
{
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier of the user in the membership database.
    /// </summary>
    public string IdentityUserId { get; set; }

    /// <summary>
    /// The new display name of the user.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// The list of roles to be assigned to the user.
    /// This value is populated only an administrator updates a user.
    /// </summary>
    public IList<Role> Roles { get; set; } = new List<Role>();
}