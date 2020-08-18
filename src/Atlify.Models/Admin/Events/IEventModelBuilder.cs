using System;
using System.Threading.Tasks;

namespace Atlify.Models.Admin.Events
{
    public interface IEventModelBuilder
    {
        Task<TargetEventsComponentModel> BuildTargetModelAsync(Guid siteId, Guid id);
    }
}
