using Atles.Core.Queries;

namespace Atles.Domain.Rules.Categories
{
    public class IsCategoryNameUnique : QueryBase<bool>
    {
        public string Name { get; set; }
        public Guid? Id { get; set; }
    }
}
