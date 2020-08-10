using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Models.Admin.Site
{
    public class SettingsPageModel
    {
        public SiteModel Site { get; set; } = new SiteModel();
        public IList<string> Themes { get; set; } = new List<string>();
        public IList<string> Css { get; set; } = new List<string>();

        public class SiteModel
        {
            public Guid Id { get; set; }

            [Required]
            [StringLength(50)]
            public string Title { get; set; }

            [Required]
            [StringLength(250)]
            public string Theme { get; set; }

            [Required]
            [StringLength(250)]
            public string Css { get; set; }
        }
    }
}
