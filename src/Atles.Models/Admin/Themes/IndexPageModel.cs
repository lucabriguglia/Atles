using System;
using System.Collections.Generic;

namespace Atles.Domain.Themes
{
    public class IndexPageModel
    {
        public IList<IndexPageModel.ThemeModel> Themes { get; set; } = new List<IndexPageModel.ThemeModel>();

        public class ThemeModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}