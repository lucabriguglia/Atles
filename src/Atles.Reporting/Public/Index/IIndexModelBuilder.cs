using System;
using System.Threading.Tasks;

namespace Atles.Models.Public.Index
{
    public interface IIndexModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId);
    }
}