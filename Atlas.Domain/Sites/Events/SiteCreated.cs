namespace Atlas.Domain.Sites.Events
{
    public class SiteCreated : EventBase
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}
