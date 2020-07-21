namespace Atlas.Domain.ForumGroups.Events
{
    public class ForumGroupReordered : EventBase
    {
        public int SortOrder { get; set; }
    }
}
