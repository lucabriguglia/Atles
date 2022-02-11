using System;
using System.Collections.Generic;

namespace Atles.Domain.Models;

/// <summary>
/// UserRank
/// </summary>
public class UserRank
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int Order { get; private set; }
    public string Badge { get; private set; }
    public string Role { get; private set; }

    public IReadOnlyCollection<UserLevel> UserLevels => _UserLevels;
    private readonly List<UserLevel> _UserLevels = new();

    public UserRank(string name, int order, string badge, string role)
    {
        Id = Guid.NewGuid();
        Name = name;
        Order = order;
        Badge = badge;
        Role = role;
    }
}

public class UserBadge
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int SortOrder { get; private set; }
    public string Image { get; private set; }
    public string Role { get; private set; }

    public IReadOnlyCollection<UserLevel> UserLevels => _UserLevels;
    private readonly List<UserLevel> _UserLevels = new();

    public UserBadge(string name, int sortOrder, string image, string role)
    {
        Id = Guid.NewGuid();
        Name = name;
        SortOrder = sortOrder;
        Image = image;
        Role = role;
    }
}

public class UserBadge2
{
    public Guid UserBadgeId { get; private set; }
    public UserBadgeType Type { get; private set; }
    public int Count { get; private set; }
    public string Image { get; private set; }

    public virtual UserBadge UserBadge { get; set; }

    public UserBadge2(UserBadgeType type, int count, string image = null)
    {
        Type = type;
        Count = count;
        Image = image;
    }
}

public enum UserBadgeType
{
    /// <summary>
    /// Number of total posts (topics and replies).
    /// </summary>
    Posts = 1,

    /// <summary>
    /// Number of topics.
    /// </summary>
    Topics = 2,

    /// <summary>
    /// Number of replies.
    /// </summary>
    Replies = 3,

    /// <summary>
    /// Number of accepted answers.
    /// </summary>
    Answers = 4
}