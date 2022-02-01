using Atles.Core.Queries;

namespace Atles.Domain.Rules.Forums
{
    public class IsForumNameUnique : QueryBase<bool>
    {
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? Id { get; set; }
    }
}
