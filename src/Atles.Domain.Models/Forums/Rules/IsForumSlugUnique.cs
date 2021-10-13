using Atles.Infrastructure.Queries;
using System;

namespace Atles.Domain.Forums.Rules
{
    public class IsForumSlugUnique : QueryBase<bool>
    {
        public string Slug { get; set; }
        public Guid? Id { get; set; }
    }
}
