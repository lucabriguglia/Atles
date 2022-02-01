using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request to update a user.
    /// </summary>
    [DocRequest(typeof(User))]
    public class UpdateUser : CommandBase
    {
        /// <summary>
        /// The new display name of the user.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The list of roles to be assigned to the user.
        /// This value is populated only an administrator updates a user.
        /// </summary>
        public IList<string> Roles { get; set; } = null;
    }
}
