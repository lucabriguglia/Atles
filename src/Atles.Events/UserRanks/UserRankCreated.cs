using Atles.Core.Events;
using Atles.Domain;

namespace Atles.Events.UserRanks
{
    /// <summary>
    /// Event published when a user rank is created.
    /// </summary>
    public class UserRankCreated : EventBase
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
        /// The user rank rules
        /// </summary>
        public ICollection<UserRankRule> UserRankRules { get; set; } = new List<UserRankRule>();
    }
}
