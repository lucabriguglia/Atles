using Atlas.Shared.Models.Admin.Events;
using System;
using System.Threading.Tasks;

namespace Atlas.Shared
{
    public interface IEventModelBuilder
    {
        Task<TargetModel> BuildTargetModelAsync(Guid siteId, Guid id);
    }
}
