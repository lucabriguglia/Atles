namespace Atlas.Models
{
    public class QueryOptions
    {
        private const int DefaultPageSize = 10;

        public string Search { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;

        public int Skip => (CurrentPage - 1) * PageSize;

        public QueryOptions()
        {
            
        }

        public QueryOptions(int? currentPage, int? pageSize = DefaultPageSize)
        {
            CurrentPage = SetCurrentPage(currentPage);
            PageSize = pageSize ?? DefaultPageSize;
        }

        public QueryOptions(string search, int? currentPage, int? pageSize = DefaultPageSize)
        {
            Search = search;
            CurrentPage = SetCurrentPage(currentPage);
            PageSize = pageSize ?? DefaultPageSize;
        }

        private static int SetCurrentPage(int? currentPage) => currentPage == null || currentPage.Value < 1 ? 1 : currentPage.Value;
    }
}
