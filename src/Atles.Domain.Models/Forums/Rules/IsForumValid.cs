using Atles.Infrastructure.Queries;
using System;

namespace Atles.Domain.Forums.Rules
{
    public class IsForumValid : QueryBase<bool>
    {
        public Guid Id { get; set; }
    }
}
