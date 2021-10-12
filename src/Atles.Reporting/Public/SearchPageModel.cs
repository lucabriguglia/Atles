namespace Atles.Models.Public.Search
{
    public class SearchPageModel
    {
        public PaginatedData<SearchPostModel> Posts { get; set; } = new PaginatedData<SearchPostModel>();
    }
}
