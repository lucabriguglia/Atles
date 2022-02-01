using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request to create a new user.
    /// This request is sent right after a user completes the registration in the site.
    /// If a user is already registered but a profile does not exist yet, this request is sent the first time the user logs in.
    /// </summary>
    [DocRequest(typeof(User))]
    public class CreateUser : CommandBase
    {
        public Guid CreateUserId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The unique identifier of the user in the membership database.
        /// </summary>
        public string IdentityUserId { get; set; }

        /// <summary>
        /// The email of the user in the membership database.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// A value indicating whether the status of the new user needs to be set as active (true) or pending (false).
        /// When a user registers through the site the value is false because the user needs to confirm the email address before being set as active.
        /// When an administrator creates a user through the admin are, this value is true and the user is immediately active.
        /// </summary>
        public bool Confirm { get; set; }
    }
}
