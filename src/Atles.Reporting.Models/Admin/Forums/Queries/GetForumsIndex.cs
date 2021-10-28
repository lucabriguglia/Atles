using System;
using Atles.Infrastructure.Queries;

namespace Atles.Reporting.Models.Admin.Forums.Queries
{
    public class GetForumsIndex : QueryBase<IndexPageModel>
    {
        public Guid? CategoryId { get; set; }
    }
}
