namespace Atles.Domain.Models;

public class UserLevelCount
{
    public UserLevelCountType Type { get; private set; }
    public int Count { get; private set; }
    public string Badge { get; private set; }

    public UserLevelCount(UserLevelCountType type, int count, string badge = null)
    {
        Type = type;
        Count = count;
        Badge = badge;
    }
}