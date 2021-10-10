using Atles.Infrastructure.Queries;
using Atles.Models.Admin.Users;
using System;

namespace Atles.Reporting.Admin.Users.Queries
{
    public class GetUserEditForm : QueryBase<EditPageModel>
    {
        public Guid? Id { get; set; }
        public string IdentityUserId { get; set; }
    }
}
