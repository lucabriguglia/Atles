using System;
using Atles.Infrastructure.Queries;

namespace Atles.Domain.Models.Forums.Rules
{
    public class IsForumSlugUnique : QueryBase<bool>
    {
        public string Slug { get; set; }
        public Guid? Id { get; set; }
    }
}
