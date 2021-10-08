using Atles.Infrastructure.Queries;
using Atles.Models.Admin.Forums;
using System;

namespace Atles.Reporting.Admin.Forums
{
    public class GetForumCreateForm : QueryBase<FormComponentModel>
    {
        public Guid? CategoryId { get; set; }
    }
}
