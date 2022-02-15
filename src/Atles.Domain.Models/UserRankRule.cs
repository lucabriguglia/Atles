using System;

namespace Atles.Domain.Models;

/// <summary>
/// User rank rule
/// </summary>
public class UserRankRule
{
    /// <summary>
    /// The unique identifier of the user rank the rule belongs to
    /// </summary>
    public Guid UserRankId { get; private set; }

    /// <summary>
    /// The rule type
    /// </summary>
    public UserRankRuleType Type { get; private set; }

    /// <summary>
    /// The name of the user rank rule
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// The description of the user rank rule
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// The count of the user rank rule
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// The badge of the user rank rule
    /// </summary>
    public string Badge { get; private set; }

    /// <summary>
    /// The user rank the rule belongs to
    /// </summary>
    public virtual UserRank UserRank { get; set; }

    /// <summary>
    /// New empty instance
    /// </summary>
    public UserRankRule()
    {
    }

    /// <summary>
    /// New instance
    /// </summary>
    /// <param name="userRankId"></param>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="count"></param>
    /// <param name="badge"></param>
    public UserRankRule(Guid userRankId, UserRankRuleType type, string name, string description, int count, string badge)
    {
        Name = name;
        Description = description;
        UserRankId = userRankId;
        Type = type;
        Count = count;
        Badge = badge;
    }
}