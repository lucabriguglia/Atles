using System.Collections.Generic;

namespace Atles.Domain.Models;

/// <summary>
/// User Level
/// </summary>
public class UserLevel
{
    public string Name { get; private set; }
    public int Level { get; private set; }
    public string Badge { get; private set; }

    public IReadOnlyCollection<UserLevelCount> UserLevelCounts => _userLevelCounts;
    private readonly List<UserLevelCount> _userLevelCounts = new();

    public UserLevel(string name, int level, string badge = null)
    {
        Name = name;
        Level = level;
        Badge = badge;
    }
}