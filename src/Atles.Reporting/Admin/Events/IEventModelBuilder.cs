using System;
using System.Threading.Tasks;

namespace Atles.Models.Admin.Events
{
    public interface IEventModelBuilder
    {
        Task<TargetEventsComponentModel> BuildTargetModelAsync(Guid siteId, Guid id);
    }
}
