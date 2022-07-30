using Atles.Core.Queries;
using Atles.Models.Admin.Users;

namespace Atles.Queries.Admin
{
    public class GetUserEditForm : QueryBase<EditPageModel>
    {
        public Guid? Id { get; set; }
        public string IdentityUserId { get; set; }
    }
}
