using Atles.Core.Queries;
using Atles.Models.Admin.Users;

namespace Atles.Queries.Admin
{
    public class GetUserEditForm : QueryBase<EditUserPageModel>
    {
        public Guid? Id { get; set; }
        public string IdentityUserId { get; set; }
    }
}
