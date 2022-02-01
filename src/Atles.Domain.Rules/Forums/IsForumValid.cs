using Atles.Core.Queries;

namespace Atles.Domain.Rules.Forums
{
    public class IsForumValid : QueryBase<bool>
    {
        public Guid Id { get; set; }
    }
}
