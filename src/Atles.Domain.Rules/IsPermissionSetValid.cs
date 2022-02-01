using Atles.Infrastructure.Queries;

namespace Atles.Domain.Rules
{
    public class IsPermissionSetValid : QueryBase<bool>
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
