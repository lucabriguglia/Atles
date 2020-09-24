using System;

namespace Atles.Domain.Themes.Commands
{
    /// <summary>
    /// Request to update a Theme.
    /// </summary>
    public class UpdateTheme : CommandBase
    {
        /// <summary>
        /// The unique identifier of the Theme to update.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The new name for the Theme. This value is required.
        /// </summary>
        public string Name { get; set; }
    }
}