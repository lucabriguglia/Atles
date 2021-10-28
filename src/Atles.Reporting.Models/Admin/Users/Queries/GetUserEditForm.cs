using System;
using Atles.Infrastructure.Queries;

namespace Atles.Reporting.Models.Admin.Users.Queries
{
    public class GetUserEditForm : QueryBase<EditPageModel>
    {
        public Guid? Id { get; set; }
        public string IdentityUserId { get; set; }
    }
}
