using System;

namespace Atles.Domain.Themes
{
    /// <summary>
    /// Theme
    /// </summary>
    public class Theme
    {
        /// <summary>
        /// The unique identifier of the site which the Theme belongs to.
        /// </summary>
        public Guid SiteId { get; private set; }

        /// <summary>
        /// The unique identifier of the Theme.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The name of the Theme.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The status of the Theme.
        /// Value can be either published or deleted.
        /// </summary>
        public ThemeStatus Status { get; private set; }

        /// <summary>
        /// Create an empty Theme.
        /// </summary>
        public Theme()
        {
        }

        /// <summary>
        /// Create a new Theme with the given values.
        /// The default status is published.
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="name"></param>
        public static Theme CreateNew(Guid siteId, string name)
        {
            return new Theme(Guid.NewGuid(), siteId, name);
        }

        /// <summary>
        /// Create a new Theme with the given values.
        /// The default status is published.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="siteId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Theme CreateNew(Guid id, Guid siteId, string name)
        {
            return new Theme(id, siteId, name);
        }

        private Theme(Guid id, Guid siteId, string name)
        {
            Id = id;
            SiteId = siteId;
            Name = name;
            Status = ThemeStatus.Published;
        }

        /// <summary>
        /// Update the details a the Theme with the given values.
        /// </summary>
        /// <param name="name"></param>
        public void UpdateDetails(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Set the status as deleted.
        /// </summary>
        public void Delete()
        {
            Status = ThemeStatus.Deleted;
        }

        /// <summary>
        /// Set the status as published.
        /// </summary>
        public void Restore()
        {
            Status = ThemeStatus.Published;
        }
    }
}
