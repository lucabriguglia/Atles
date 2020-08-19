using System;
using System.Threading.Tasks;

namespace Atlas.Models.Admin.Events
{
    public interface IEventModelBuilder
    {
        Task<TargetEventsComponentModel> BuildTargetModelAsync(Guid siteId, Guid id);
    }
}
