using Atles.Infrastructure.Queries;
using System;

namespace Atles.Domain.PermissionSets.Rules
{
    public class IsPermissionSetValid : QueryBase<bool>
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
