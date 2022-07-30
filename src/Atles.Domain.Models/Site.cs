using System;
using System.Collections.Generic;
using Docs.Attributes;

namespace Atles.Domain
{
    /// <summary>
    /// Site
    /// </summary>
    [DocTarget(Consts.DocsContextForum)]
    public class Site
    {
        /// <summary>
        /// The unique identifier of the site.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The name of the site.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The title of the site.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// The name of the theme to use in the public site.
        /// </summary>
        public string PublicTheme { get; private set; }

        /// <summary>
        /// The Cascading Style Sheet to use in the public site.
        /// </summary>
        public string PublicCss { get; private set; }

        /// <summary>
        /// The name of the theme to use in the admin area.
        /// </summary>
        public string AdminTheme { get; private set; }

        /// <summary>
        /// The Cascading Style Sheet to use in the admin area.
        /// </summary>
        public string AdminCss { get; private set; }

        /// <summary>
        /// The ISO code for the language of the site (e.g. en or en-GB).
        /// </summary>
        public string Language { get; private set; }

        /// <summary>
        /// The content of the privacy page.
        /// </summary>
        public string Privacy { get; private set; }

        /// <summary>
        /// The content of the terms page.
        /// </summary>
        public string Terms { get; private set; }

        /// <summary>
        /// Optional script to be added at the beginning of the head section of the HTML document.
        /// It could be used for example to add the Google Analytics script.
        /// </summary>
        public string HeadScript { get; private set; }

        /// <summary>
        /// The list of forum categories.
        /// </summary>
        public virtual ICollection<Category> Categories { get; set; }

        /// <summary>
        /// Creates an empty site.
        /// </summary>
        public Site()
        {
        }

        /// <summary>
        /// Creates a new site with the given values.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="title"></param>
        public Site(string name, string title)
        {
            New(Guid.NewGuid(), name, title);
        }

        /// <summary>
        /// Creates a new site with the given values including the unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="title"></param>
        public Site(Guid id, string name, string title)
        {
            New(id, name, title);
        }

        private void New(Guid id, string name, string title)
        {
            Id = id;
            Name = name;
            Title = title;
            PublicTheme = "Default";
            PublicCss = "public.css";
            AdminTheme = "Default";
            AdminCss = "admin.css";
            Language = "en";
            Privacy = "Privacy statement...";
            Terms = "Terms of service...";
        }

        /// <summary>
        /// Updates the details of the site.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="theme"></param>
        /// <param name="css"></param>
        /// <param name="language"></param>
        /// <param name="privacy"></param>
        /// <param name="terms"></param>
        /// <param name="headScript"></param>
        public void UpdateDetails(string title, 
            string theme, 
            string css, 
            string language, 
            string privacy, 
            string terms,
            string headScript)
        {
            Title = title;
            PublicTheme = theme;
            PublicCss = css;
            Language = language;
            Privacy = privacy;
            Terms = terms;
            HeadScript = headScript;
        }
    }
}
