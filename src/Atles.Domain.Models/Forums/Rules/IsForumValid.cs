using System;
using Atles.Infrastructure.Queries;

namespace Atles.Domain.Models.Forums.Rules
{
    public class IsForumValid : QueryBase<bool>
    {
        public Guid Id { get; set; }
    }
}
