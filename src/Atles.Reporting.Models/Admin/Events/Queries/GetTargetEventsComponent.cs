using System;
using Atles.Infrastructure.Queries;

namespace Atles.Reporting.Models.Admin.Events.Queries
{
    public class GetTargetEventsComponent : QueryBase<TargetEventsComponentModel>
    {
        public Guid Id { get; set; }
    }
}
