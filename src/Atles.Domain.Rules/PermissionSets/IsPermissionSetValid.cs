using Atles.Core.Queries;

namespace Atles.Domain.Rules.PermissionSets
{
    public class IsPermissionSetValid : QueryBase<bool>
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
