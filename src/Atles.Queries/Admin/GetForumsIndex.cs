using Atles.Core.Queries;
using Atles.Models.Admin.Forums;

namespace Atles.Queries.Admin
{
    public class GetForumsIndex : QueryBase<IndexPageModel>
    {
        public Guid? CategoryId { get; set; }
    }
}
