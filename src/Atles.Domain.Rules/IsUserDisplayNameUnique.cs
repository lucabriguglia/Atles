using Atles.Infrastructure.Queries;

namespace Atles.Domain.Rules
{
    public class IsUserDisplayNameUnique : QueryBase<bool>
    {
        public string DisplayName { get; set; }
        public Guid? Id { get; set; }
    }
}
