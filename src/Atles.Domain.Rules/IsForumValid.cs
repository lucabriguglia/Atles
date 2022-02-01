using Atles.Core.Queries;

namespace Atles.Domain.Rules
{
    public class IsForumValid : QueryBase<bool>
    {
        public Guid Id { get; set; }
    }
}
