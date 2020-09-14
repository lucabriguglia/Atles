using System;
using System.Threading.Tasks;

namespace Atlas.Models.Public.Index
{
    public interface IIndexModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId);
    }
}