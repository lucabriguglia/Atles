namespace Atlas.Domain.Categories.Events
{
    public class CategoryReordered : EventBase
    {
        public int SortOrder { get; set; }
    }
}
