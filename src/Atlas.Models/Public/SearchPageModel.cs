namespace Atlas.Models.Public
{
    public class SearchPageModel
    {
        public PaginatedData<SearchPostModel> Posts { get; set; } = new PaginatedData<SearchPostModel>();
    }
}
