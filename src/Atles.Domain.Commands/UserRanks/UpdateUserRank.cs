using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands.UserRanks
{
    /// <summary>
    /// Request to update a user rank.
    /// </summary>
    [DocRequest(typeof(UserRank))]
    public class UpdateUserRank : CommandBase
    {
        /// <summary>
        /// The id of the user rank
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the user rank
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the user rank
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The badge of the user rank
        /// </summary>
        public string Badge { get; set; }

        /// <summary>
        /// The role automatically assigned when a user reaches the rank
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// The status of the user rank
        /// </summary>
        public UserRankStatusType Status { get; set; }

        /// <summary>
        /// The rules of the user rank.
        /// </summary>
        public ICollection<UserRankRuleCommand> UserRankRules { get; set; } = new List<UserRankRuleCommand>();
    }
}
