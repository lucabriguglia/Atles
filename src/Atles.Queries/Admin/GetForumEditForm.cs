using Atles.Core.Queries;
using Atles.Models.Admin.Forums;

namespace Atles.Queries.Admin
{
    public class GetForumEditForm : QueryBase<CreateForumFormModel>
    {
        public Guid Id { get; set; }
    }
}
