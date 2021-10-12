using Atles.Infrastructure.Queries;
using Atles.Models.Admin.Events;
using System;

namespace Atles.Reporting.Admin.Events.Queries
{
    public class GetTargetEventsComponent : QueryBase<TargetEventsComponentModel>
    {
        public Guid Id { get; set; }
    }
}
