using Atles.Core.Queries;
using Atles.Models.Admin.Events;

namespace Atles.Queries.Admin
{
    public class GetTargetEventsComponent : QueryBase<TargetEventsComponentModel>
    {
        public Guid Id { get; set; }
    }
}
