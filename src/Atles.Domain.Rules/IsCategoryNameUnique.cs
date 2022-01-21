using System;
using Atles.Infrastructure.Queries;

namespace Atles.Domain.Rules
{
    public class IsCategoryNameUnique : QueryBase<bool>
    {
        public string Name { get; set; }
        public Guid? Id { get; set; }
    }
}
