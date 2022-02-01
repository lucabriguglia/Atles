using Atles.Core.Queries;

namespace Atles.Domain.Rules.PermissionSets
{
    public class IsPermissionSetNameUnique : QueryBase<bool>
    {
        public string Name { get; set; }
        public Guid? Id { get; set; }
    }
}
