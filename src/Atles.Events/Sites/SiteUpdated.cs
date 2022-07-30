using Atles.Core.Events;

namespace Atles.Events.Sites
{
    /// <summary>
    /// Event published when a site is updated.
    /// </summary>
    public class SiteUpdated : EventBase
    {
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

        /// <summary>
        /// The new language ISO code of the site.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The new content of the privacy page.
        /// </summary>
        public string Privacy { get; set; }

        /// <summary>
        /// The new content of the terms page.
        /// </summary>
        public string Terms { get; set; }

        /// <summary>
        /// The script for the head section of the HTML document.
        /// </summary>
        public string HeadScript { get; set; }
    }
}
