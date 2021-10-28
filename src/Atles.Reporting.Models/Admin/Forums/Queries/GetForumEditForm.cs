using System;
using Atles.Infrastructure.Queries;

namespace Atles.Reporting.Models.Admin.Forums.Queries
{
    public class GetForumEditForm : QueryBase<FormComponentModel>
    {
        public Guid Id { get; set; }
    }
}
