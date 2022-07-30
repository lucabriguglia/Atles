using Atles.Domain;

namespace Atles.Commands.UserRanks;

public static class UserRankRuleCommandExtensions
{
    public static ICollection<UserRankRule> ToDomainRules(this ICollection<UserRankRuleCommand> models)
    {
        var result = new List<UserRankRule>();

        foreach (var rule in models)
        {
            result.Add(new UserRankRule(rule.Type, rule.Name, rule.Description, rule.Count, rule.Badge));
        }

        return result;
    }
}