using System;

namespace Atles.Domain.Themes.Commands
{
    /// <summary>
    /// Request to create a new Theme.
    /// </summary>
    public class CreateTheme : CommandBase
    {
        /// <summary>
        /// The unique identifier for the new Theme. If not specified, a random one is assigned.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The name for the new Theme. This value is required.
        /// </summary>
        public string Name { get; set; }
    }
}