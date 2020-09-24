using System.Threading.Tasks;
using Atles.Domain.Themes.Commands;

namespace Atles.Domain.Themes
{
    /// <summary>
    /// IThemeService
    /// </summary>
    public interface IThemeService
    {
        /// <summary>
        /// CreateAsync
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task CreateAsync(CreateTheme command);

        /// <summary>
        /// UpdateAsync
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task UpdateAsync(UpdateTheme command);

        /// <summary>
        /// DeleteAsync
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task DeleteAsync(DeleteTheme command);

        /// <summary>
        /// RestoreAsync
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task RestoreAsync(RestoreTheme command);
    }
}