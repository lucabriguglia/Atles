using Atles.Reporting.Models.Shared;

namespace Atles.Reporting.Models.Public
{
    public class SearchPageModel
    {
        public PaginatedData<SearchPostModel> Posts { get; set; } = new PaginatedData<SearchPostModel>();
    }
}
