using Atles.Infrastructure.Queries;
using System;

namespace Atles.Domain.Forums.Rules
{
    public class IsForumNameUnique : QueryBase<bool>
    {
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? Id { get; set; }
    }
}
