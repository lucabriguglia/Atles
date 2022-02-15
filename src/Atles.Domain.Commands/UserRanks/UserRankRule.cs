using Atles.Domain.Models;

namespace Atles.Domain.Commands.UserRanks;

public class UserRankRuleCommand
{
    public UserRankRuleType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Count { get; set; }
    public string Badge { get; set; }
}

public static class UserRankRuleCommandExtensions
{
    public static ICollection<UserRankRule> ToDomainPermissions(this ICollection<UserRankRuleCommand> models)
    {
        var result = new List<UserRankRule>();

        foreach (var rule in models)
        {
            result.Add(new UserRankRule(rule.Type));
        }

        return result;
    }
}