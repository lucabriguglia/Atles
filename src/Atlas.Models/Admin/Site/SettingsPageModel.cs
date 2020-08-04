using System;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Models.Admin.Site
{
    public class SettingsPageModel
    {
        public Guid SiteId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }
    }
}
