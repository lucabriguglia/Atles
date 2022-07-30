using Atles.Core.Commands;
using Atles.Domain;

namespace Atles.Commands.UserRanks
{
    /// <summary>
    /// Request to create a new user rank.
    /// </summary>
    public class CreateUserRank : CommandBase
    {
        /// <summary>
        /// The name of the user rank
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the user rank
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The sort order of the user rank
        /// </summary>
        public int SortOrder { get; set; }

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
        /// The rules of the new user rank.
        /// </summary>
        public ICollection<UserRankRuleCommand> UserRankRules { get; set; } = new List<UserRankRuleCommand>();
    }
}
