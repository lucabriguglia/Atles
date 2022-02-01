using System;
using Atles.Core.Queries;

namespace Atles.Reporting.Models.Admin.Forums.Queries
{
    public class GetForumCreateForm : QueryBase<FormComponentModel>
    {
        public Guid? CategoryId { get; set; }
    }
}
