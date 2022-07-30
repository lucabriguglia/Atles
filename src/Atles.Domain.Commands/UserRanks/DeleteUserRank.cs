using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands.UserRanks;

/// <summary>
/// Request to delete a user rank.
/// </summary>
[DocRequest(typeof(UserRank))]
public class DeleteUserRank : CommandBase
{
    /// <summary>
    /// The id of the user rank
    /// </summary>
    public Guid UserRankId { get; set; }
}