using Atles.Core.Queries;
using Atles.Models.Admin.Forums;

namespace Atles.Queries.Admin
{
    public class GetForumsIndex : QueryBase<ForumsPageModel>
    {
        public Guid? CategoryId { get; set; }
    }
}
