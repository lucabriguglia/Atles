using System;
using System.Collections.Generic;

namespace Atles.Domain;

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
    /// Unique identifier of the site
    /// </summary>
    public Guid SiteId { get; private set; }

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
    /// The status of the user rank
    /// </summary>
    public UserRankStatusType Status { get; private set; }

    /// <summary>
    /// The rules associated with the user rank
    /// </summary>
    public IReadOnlyCollection<UserRankRule> UserRankRules => _userRankRules;
    private readonly List<UserRankRule> _userRankRules = new();

    /// <summary>
    /// Reference to the Site the user rank belongs to.
    /// </summary>
    public virtual Site Site { get; set; }

    /// <summary>
    /// New empty instance
    /// </summary>
    public UserRank()
    {
    }

    /// <summary>
    /// New instance
    /// </summary>
    /// <param name="siteId"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="sortOrder"></param>
    /// <param name="badge"></param>
    /// <param name="role"></param>
    /// <param name="status"></param>
    /// <param name="rules"></param>
    public UserRank(
        Guid siteId,
        string name,
        string description,
        int sortOrder,
        string badge,
        string role,
        UserRankStatusType status,
        IEnumerable<UserRankRule> rules)
    {
        Id = Guid.NewGuid();
        SiteId = siteId;

        UpdateInternal(name, description, sortOrder, badge, role, status, rules);
    }

    /// <summary>
    /// Update user rank details
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="badge"></param>
    /// <param name="role"></param>
    /// <param name="status"></param>
    /// <param name="rules"></param>
    public void UpdateDetails(
        string name,
        string description,
        string badge,
        string role,
        UserRankStatusType status,
        IEnumerable<UserRankRule> rules)
    {
        UpdateInternal(name, description, SortOrder, badge, role, status, rules);
    }

    private void UpdateInternal(
        string name,
        string description,
        int sortOrder,
        string badge,
        string role,
        UserRankStatusType status,
        IEnumerable<UserRankRule> rules)
    {
        Name = name;
        Description = description;
        SortOrder = sortOrder;
        Badge = badge;
        Role = role;
        Status = status;

        _userRankRules.Clear();

        foreach (var rule in rules)
        {
            _userRankRules.Add(new UserRankRule(rule.Type, rule.Name, rule.Description, rule.Count, rule.Badge));
        }
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

    /// <summary>
    /// Set the status as deleted.
    /// </summary>
    public void Delete()
    {
        Status = UserRankStatusType.Deleted;
    }
}