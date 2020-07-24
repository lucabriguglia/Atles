using System;
using System.Threading.Tasks;
using Atlas.Shared.Admin.Events.Models;

namespace Atlas.Shared
{
    public interface IEventModelBuilder
    {
        Task<TargetEventsComponentModel> BuildTargetModelAsync(Guid siteId, Guid id);
    }
}
