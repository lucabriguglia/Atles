using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atles.Reporting.Models.Admin.Sites
{
    public class SettingsPageModel
    {
        public SiteModel Site { get; set; } = new SiteModel();
        public IList<string> Themes { get; set; } = new List<string>();
        public IList<string> Css { get; set; } = new List<string>();
        public IList<string> Languages { get; set; } = new List<string>();

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

            [Required]
            [StringLength(10)]
            public string Language { get; set; }

            [Required]
            public string Privacy { get; set; }

            [Required]
            public string Terms { get; set; }

            public string HeadScript { get; set; }
        }
    }
}
