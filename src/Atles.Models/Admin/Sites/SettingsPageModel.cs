namespace Atles.Models.Admin.Sites;

public class SettingsPageModel
{
    public SiteModel Site { get; set; } = new();
    public IList<string> Themes { get; set; } = new List<string>();
    public IList<string> Css { get; set; } = new List<string>();
    public IList<string> Languages { get; set; } = new List<string>();

    public class SiteModel : SiteFormModel
    {
        public string Title { get; set; }
        public string Theme { get; set; }
        public string Css { get; set; }
        public string Language { get; set; }
        public string Privacy { get; set; }
        public string Terms { get; set; }
        public string HeadScript { get; set; }
    }
}
