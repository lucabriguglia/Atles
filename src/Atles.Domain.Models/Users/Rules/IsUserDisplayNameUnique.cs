using Atles.Infrastructure.Queries;
using System;

namespace Atles.Domain.Categories.Rules
{
    public class IsUserDisplayNameUnique : QueryBase<bool>
    {
        public string DisplayName { get; set; }
        public Guid? Id { get; set; }
    }
}
