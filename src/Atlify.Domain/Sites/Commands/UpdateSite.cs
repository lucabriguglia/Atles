namespace Atlify.Domain.Sites.Commands
{
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
