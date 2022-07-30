using System;
using Atles.Core.Queries;

namespace Atles.Reporting.Models.Admin.Forums.Queries
{
    public class GetForumsIndex : QueryBase<IndexPageModel>
    {
        public Guid? CategoryId { get; set; }
    }
}
