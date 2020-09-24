using System;

namespace Atles.Domain.Themes.Commands
{
    /// <summary>
    /// Request to restore a Theme.
    /// </summary>
    public class RestoreTheme : CommandBase
    {
        /// <summary>
        /// The unique identifier of the Theme to restore.
        /// </summary>
        public Guid Id { get; set; }
    }
}