using Atles.Core.Queries;

namespace Atles.Domain.Rules.UserRanks
{
    public class IsUserRankNameUnique : QueryBase<bool>
    {
        public string Name { get; set; }
        public Guid? Id { get; set; }
    }
}
