using Atles.Infrastructure.Queries;
using Atles.Models.Admin.Forums;
using System;

namespace Atles.Reporting.Admin.Forums
{
    public class GetForumsIndex : QueryBase<IndexPageModel>
    {
        public Guid? CategoryId { get; set; }
    }
}
