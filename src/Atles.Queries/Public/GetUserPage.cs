using Atles.Core.Queries;
using Atles.Models.Public;

namespace Atles.Queries.Public
{
    public class GetUserPage : QueryBase<UserPageModel>
    {
        public Guid UserId { get; set; }
        public IEnumerable<Guid> AccessibleForumIds { get; set; }
    }
}
