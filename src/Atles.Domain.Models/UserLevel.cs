using System;

namespace Atles.Domain.Models;

public class UserLevel
{
    public Guid UserRankId { get; private set; }
    public UserLevelType Type { get; private set; }
    public int Count { get; private set; }
    public string Badge { get; private set; }

    public virtual UserRank UserRank { get; set; }

    public UserLevel(UserLevelType type, int count, string badge = null)
    {
        Type = type;
        Count = count;
        Badge = badge;
    }
}