using System;
using Atles.Core.Queries;

namespace Atles.Reporting.Models.Admin.Forums.Queries
{
    public class GetForumEditForm : QueryBase<FormComponentModel>
    {
        public Guid Id { get; set; }
    }
}
