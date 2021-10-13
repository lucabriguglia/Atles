using Atles.Infrastructure.Queries;
using System;

namespace Atles.Domain.Categories.Rules
{
    public class IsCategoryNameUnique : QueryBase<bool>
    {
        public string Name { get; set; }
        public Guid? Id { get; set; }
    }
}
