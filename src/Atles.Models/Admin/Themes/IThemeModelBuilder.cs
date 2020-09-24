using System;
using System.Threading.Tasks;

namespace Atles.Domain.Themes
{
    /// <summary>
    /// IThemeModelBuilder
    /// </summary>
    public interface IThemeModelBuilder
    {
        /// <summary>
        /// BuildIndexPageModelAsync
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId);

        /// <summary>
        /// BuildFormModelAsync
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FormComponentModel> BuildFormModelAsync(Guid siteId, Guid? id = null);
    }
}