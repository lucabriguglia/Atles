using Atles.Core.Events;

namespace Atles.Events.Sites
{
    /// <summary>
    /// Event published when a site is created.
    /// </summary>
    public class SiteCreated : EventBase
    {
        /// <summary>
        /// The name of the site.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The new tile of the site.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The new theme for the public site.
        /// </summary>
        public string PublicTheme { get; set; }

        /// <summary>
        /// The new Cascading Style Sheet for the public site.
        /// </summary>
        public string PublicCss { get; set; }

        /// <summary>
        /// The new theme for the public site.
        /// </summary>
        public string AdminTheme { get; set; }

        /// <summary>
        /// The new Cascading Style Sheet for the public site.
        /// </summary>
        public string AdminCss { get; set; }
    }
}
