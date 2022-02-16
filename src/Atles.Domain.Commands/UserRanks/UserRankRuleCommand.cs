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