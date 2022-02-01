using Atles.Infrastructure.Queries;

namespace Atles.Domain.Rules
{
    public class IsForumSlugUnique : QueryBase<bool>
    {
        public string Slug { get; set; }
        public Guid? Id { get; set; }
    }
}
