using System;
using System.ComponentModel.DataAnnotations;

namespace Atles.Domain.Themes
{
    public class FormComponentModel
    {
        public ThemeModel Theme { get; set; } = new ThemeModel();

        public class ThemeModel
        {
            public Guid Id { get; set; }

            [Required]
            [StringLength(50)]
            public string Name { get; set; }
        }
    }
}