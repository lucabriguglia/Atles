using Atles.Core.Queries;
using Atles.Models.Admin.Forums;

namespace Atles.Queries.Admin
{
    public class GetForumEditForm : QueryBase<FormComponentModel>
    {
        public Guid Id { get; set; }
    }
}
