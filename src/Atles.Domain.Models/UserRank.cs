using System;
using System.Collections.Generic;

namespace Atles.Domain.Models;

/// <summary>
/// User Rank
/// </summary>
public class UserRank
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// The name of the user rank
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// The description of the user rank
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// The sort order of the user rank
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// The badge of the user rank
    /// </summary>
    public string Badge { get; private set; }

    /// <summary>
    /// The role automatically assigned when a user reaches the rank
    /// </summary>
    public string Role { get; private set; }

    /// <summary>
    /// The rules associated with the user rank
    /// </summary>
    public IReadOnlyCollection<UserRankRule> UserRankRules => _userRankRules;
    private readonly List<UserRankRule> _userRankRules = new();

    /// <summary>
    /// New empty instance
    /// </summary>
    public UserRank()
    {
    }

    /// <summary>
    /// New instance
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="sortOrder"></param>
    /// <param name="badge"></param>
    /// <param name="role"></param>
    public UserRank(string name, string description, int sortOrder, string badge, string role)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        SortOrder = sortOrder;
        Badge = badge;
        Role = role;
    }

    /// <summary>
    /// Remove all rules
    /// </summary>
    public void ClearRules()
    {
        _userRankRules.Clear();
    }

    /// <summary>
    /// Add rule
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="count"></param>
    /// <param name="badge"></param>
    public void AddRule(UserRankRuleType type, string name, string description, int count, string badge)
    {
        _userRankRules.Add(new UserRankRule(Id, type, name, description, count, badge));
    }

    /// <summary>
    /// Change the sort order by moving up 1 position.
    /// It generates an error if sort order is 1.
    /// </summary>
    /// <exception cref="ApplicationException"></exception>
    public void MoveUp()
    {
        if (SortOrder == 1)
        {
            throw new ApplicationException($"User Rank \"{Name}\" can't be moved up.");
        }

        SortOrder -= 1;
    }

    /// <summary>
    /// Change the sort order by moving down 1 position.
    /// </summary>
    public void MoveDown()
    {
        SortOrder += 1;
    }
}