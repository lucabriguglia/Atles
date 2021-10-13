using Atles.Infrastructure.Queries;
using Atles.Models.Admin.Forums;
using System;

namespace Atles.Reporting.Admin.Forums
{
    public class GetForumEditForm : QueryBase<FormComponentModel>
    {
        public Guid Id { get; set; }
    }
}
