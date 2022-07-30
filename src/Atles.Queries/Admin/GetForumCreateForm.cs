using Atles.Core.Queries;
using Atles.Models.Admin.Forums;

namespace Atles.Queries.Admin
{
    public class GetForumCreateForm : QueryBase<FormComponentModel>
    {
        public Guid? CategoryId { get; set; }
    }
}
