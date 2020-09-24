using System;

namespace Atles.Domain.Themes.Commands
{
    /// <summary>
    /// Request to delete a Theme.
    /// </summary>
    public class DeleteTheme : CommandBase
    {
        /// <summary>
        /// The unique identifier of the Theme to delete.
        /// </summary>
        public Guid Id { get; set; }
    }
}