using Atles.Core.Commands;

namespace Atles.Commands.UserRanks;

/// <summary>
/// Request to delete a user rank.
/// </summary>
public class DeleteUserRank : CommandBase
{
    /// <summary>
    /// The id of the user rank
    /// </summary>
    public Guid UserRankId { get; set; }
}