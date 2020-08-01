namespace Atlas.Domain.Forums.Events
{
    public class ForumReordered : EventBase
    {
        public int SortOrder { get; set; }
    }
}
