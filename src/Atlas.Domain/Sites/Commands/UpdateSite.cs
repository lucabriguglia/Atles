using Docs.Attributes;

namespace Atlas.Domain.Sites.Commands
{
    [DocRequest(typeof(Site))]
    public class UpdateSite : CommandBase
    {
        public string Title { get; set; }
        public string Theme { get; set; }
        public string Css { get; set; }
        public string Language { get; set; }
        public string Privacy { get; set; }
        public string Terms { get; set; }
    }
}
