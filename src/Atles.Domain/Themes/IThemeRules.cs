using System;
using System.Threading.Tasks;

namespace Atles.Domain.Themes
{
    /// <summary>
    /// IThemeRules
    /// </summary>
    public interface IThemeRules
    {
        /// <summary>
        /// IsNameUniqueAsync
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> IsNameUniqueAsync(Guid siteId, string name);

        /// <summary>
        /// IsNameUniqueAsync
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> IsNameUniqueAsync(Guid siteId, string name, Guid id);
    }
}