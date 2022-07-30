namespace Atles.Domain;

/// <summary>
/// User rank rule
/// </summary>
public class UserRankRule
{
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
    /// New empty instance
    /// </summary>
    public UserRankRule()
    {
    }

    /// <summary>
    /// New instance
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="count"></param>
    /// <param name="badge"></param>
    public UserRankRule(UserRankRuleType type, string name, string description, int count, string badge)
    {
        Type = type;
        Name = name;
        Description = description;
        Count = count;
        Badge = badge;
    }
}