using System;
using Atles.Infrastructure.Queries;

namespace Atles.Domain.Models.PermissionSets.Rules
{
    public class IsPermissionSetValid : QueryBase<bool>
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
