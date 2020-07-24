namespace Atlas.Domain
{
    public enum StatusType
    {
        Draft = 0,
        Published = 1,
        Deleted = 2
    }

    public enum EventType
    {
        Created,
        Updated,
        Deleted,
        Restored,
        Reordered
    }
}