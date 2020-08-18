using System;
using System.Threading.Tasks;

namespace Atlify.Models.Public.Index
{
    public interface IIndexModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId);
    }
}